import { TestBed, fakeAsync, tick } from '@angular/core/testing';
import { ConnectionManager } from './connection.manager';
import { LogBus } from '../log/log.bus';

describe('ConnectionManager', () => {
  let service: ConnectionManager;
  let mockWebview: any;
  let mockChrome: any;
  let mockBus: jasmine.SpyObj<LogBus>;

  beforeEach(() => {
    // Setup mock WebView2 environment
    mockWebview = {
      postMessage: jasmine.createSpy('postMessage'),
      addEventListener: jasmine.createSpy('addEventListener'),
      removeEventListener: jasmine.createSpy('removeEventListener'),
    };

    mockChrome = {
      webview: mockWebview,
    };

    (window as any).chrome = mockChrome;

    mockBus = jasmine.createSpyObj('LogBus', ['push']);

    TestBed.configureTestingModule({
      providers: [
        ConnectionManager,
        { provide: LogBus, useValue: mockBus },
      ],
    });

    service = TestBed.inject(ConnectionManager);
  });

  afterEach(() => {
    // Clean up by setting to undefined instead of deleting
    (window as any).chrome = undefined;
  });

  describe('Connection Management', () => {
    it('should create the service', () => {
      expect(service).toBeTruthy();
    });

    it('should connect to WebView2 successfully', () => {
      service.connect();

      expect(mockWebview.addEventListener).toHaveBeenCalledWith(
        'message',
        jasmine.any(Function)
      );
    });

    it('should log an error via LogBus when WebView2 is not available', fakeAsync(() => {
      (window as any).chrome = undefined;

      service.connect();
      tick(10000);

      expect(mockBus.push).toHaveBeenCalledWith(jasmine.objectContaining({
        level: 'error',
        message: jasmine.stringContaining('chrome.webview not available'),
      }));
    }));

    it('should keep the message stream open when WebView2 is not available', () => {
      (window as any).chrome = undefined;
      let streamCompleted = false;
      let streamErrored = false;

      service.messages$.subscribe({
        complete: () => { streamCompleted = true; },
        error: () => { streamErrored = true; },
      });

      service.connect();

      // Stream must remain open so that a later re-connect or test can still use it
      expect(streamCompleted).toBeFalse();
      expect(streamErrored).toBeFalse();
    });

    it('should not add a second listener when connect() is called twice', () => {
      service.connect();
      service.connect();

      expect(mockWebview.addEventListener).toHaveBeenCalledTimes(1);
    });

    it('should warn via LogBus when connect() is called twice', () => {
      service.connect();
      mockBus.push.calls.reset();
      service.connect();

      expect(mockBus.push).toHaveBeenCalledWith(jasmine.objectContaining({
        level: 'warn',
        message: jasmine.stringContaining('already connected'),
      }));
    });

    it('should disconnect from WebView2', () => {
      service.connect();
      service.disconnect();

      expect(mockWebview.removeEventListener).toHaveBeenCalledWith(
        'message',
        jasmine.any(Function)
      );
    });
  });

  describe('Message Sending', () => {
    it('should send JSON message to WebView2', () => {
      service.connect();

      const testMessage = { Action: 'TestAction', Data: 'TestData' } as any;
      service.send(testMessage);

      expect(mockWebview.postMessage).toHaveBeenCalledWith(JSON.stringify(testMessage));
    });

    it('should not send when there is no connection', () => {
      const testMessage = { Action: 'TestAction' } as any;
      service.send(testMessage);

      expect(mockWebview.postMessage).not.toHaveBeenCalled();
    });
  });

  describe('Message Receiving', () => {
    it('should emit received JSON messages', (done) => {
      service.connect();

      const testData = { Action: 'Response', Result: 'Success' };

      service.messages$.subscribe((message) => {
        expect(message).toEqual(testData as any);
        done();
      });

      // Simulate message from backend
      const messageHandler = mockWebview.addEventListener.calls
        .mostRecent()
        .args[1];
      messageHandler({ data: testData });
    });

    it('should parse stringified JSON messages', (done) => {
      service.connect();

      const testData = { Action: 'Response', Result: 'Success' };
      const stringifiedData = JSON.stringify(testData);

      service.messages$.subscribe((message) => {
        expect(message).toEqual(testData as any);
        done();
      });

      const messageHandler = mockWebview.addEventListener.calls
        .mostRecent()
        .args[1];
      messageHandler({ data: stringifiedData });
    });

    it('should ignore null or undefined messages', () => {
      service.connect();

      const receivedMessages: any[] = [];
      service.messages$.subscribe((msg) => receivedMessages.push(msg));

      const messageHandler = mockWebview.addEventListener.calls
        .mostRecent()
        .args[1];

      messageHandler({ data: null });
      messageHandler({ data: undefined });
      messageHandler({});

      expect(receivedMessages.length).toBe(0);
    });

    it('should warn via LogBus when message data is null', () => {
      service.connect();
      mockBus.push.calls.reset();

      const messageHandler = mockWebview.addEventListener.calls.mostRecent().args[1];
      messageHandler({ data: null });

      expect(mockBus.push).toHaveBeenCalledWith(jasmine.objectContaining({
        level: 'warn',
        message: jasmine.stringContaining('null/undefined'),
      }));
    });

    it('should warn via LogBus when message data is undefined', () => {
      service.connect();
      mockBus.push.calls.reset();

      const messageHandler = mockWebview.addEventListener.calls.mostRecent().args[1];
      messageHandler({ data: undefined });

      expect(mockBus.push).toHaveBeenCalledWith(jasmine.objectContaining({
        level: 'warn',
        message: jasmine.stringContaining('null/undefined'),
      }));
    });

    it('should push two warn entries for two null/undefined messages', () => {
      service.connect();
      mockBus.push.calls.reset();

      const messageHandler = mockWebview.addEventListener.calls.mostRecent().args[1];
      messageHandler({ data: null });
      messageHandler({ data: undefined });

      const warnCalls = mockBus.push.calls.all()
        .filter(c => c.args[0].level === 'warn');
      expect(warnCalls.length).toBe(2);
    });

    it('should not push a warn entry for valid messages', () => {
      service.connect();
      mockBus.push.calls.reset();

      const messageHandler = mockWebview.addEventListener.calls.mostRecent().args[1];
      messageHandler({ data: { Action: 'Test' } });

      const warnCalls = mockBus.push.calls.all().filter(c => c.args[0].level === 'warn');
      expect(warnCalls.length).toBe(0);
    });
  });

  describe('Observable Stream', () => {
    it('should provide observable stream of messages', () => {
      const observable = service.messages$;
      expect(observable).toBeDefined();
      expect(typeof observable.subscribe).toBe('function');
    });

    it('should support multiple subscribers', () => {
      service.connect();

      const messages1: any[] = [];
      const messages2: any[] = [];

      service.messages$.subscribe((msg) => messages1.push(msg));
      service.messages$.subscribe((msg) => messages2.push(msg));

      const testData = { Action: 'Test' };
      const messageHandler = mockWebview.addEventListener.calls
        .mostRecent()
        .args[1];
      messageHandler({ data: testData });

      expect(messages1).toEqual([testData]);
      expect(messages2).toEqual([testData]);
    });
  });

  describe('Edge Cases', () => {
    it('should handle disconnect without prior connection', () => {
      expect(() => service.disconnect()).not.toThrow();
    });

    it('should handle multiple connect calls', () => {
      service.connect();
      // Second call is guarded — only one listener must be registered
      service.connect();

      expect(mockWebview.addEventListener).toHaveBeenCalledTimes(1);
    });

    it('should handle complex JSON structures', (done) => {
      service.connect();

      const complexData = {
        Action: 'ComplexAction',
        Payload: {
          nested: { deep: { value: 123 } },
          array: [1, 2, 3],
        },
      };

      service.messages$.subscribe((message) => {
        expect(message).toEqual(complexData as any);
        done();
      });

      const messageHandler = mockWebview.addEventListener.calls
        .mostRecent()
        .args[1];
      messageHandler({ data: complexData });
    });

    it('should log error via LogBus when WebView2 not available on window.chrome', fakeAsync(() => {
      (window as any).chrome = {};

      service.connect();
      tick(10000);

      expect(mockBus.push).toHaveBeenCalledWith(jasmine.objectContaining({
        level: 'error',
        message: jasmine.stringContaining('chrome.webview not available'),
      }));
    }));

    it('should log error via LogBus when webview property is missing', fakeAsync(() => {
      (window as any).chrome = { somethingElse: {} };

      service.connect();
      tick(10000);

      expect(mockBus.push).toHaveBeenCalledWith(jasmine.objectContaining({
        level: 'error',
        message: jasmine.stringContaining('chrome.webview not available'),
      }));
    }));

    it('should log error via LogBus when webview is null', fakeAsync(() => {
      (window as any).chrome = { webview: null };

      service.connect();
      tick(10000);

      expect(mockBus.push).toHaveBeenCalledWith(jasmine.objectContaining({
        level: 'error',
        message: jasmine.stringContaining('chrome.webview not available'),
      }));
    }));

    it('should handle empty message data', () => {
      service.connect();

      const receivedMessages: any[] = [];
      service.messages$.subscribe((msg) => receivedMessages.push(msg));

      const messageHandler = mockWebview.addEventListener.calls
        .mostRecent()
        .args[1];

      messageHandler({ data: {} });

      expect(receivedMessages).toEqual([{}]);
    });

    it('should emit message with 0 (valid primitive)', () => {
      service.connect();

      const receivedMessages: any[] = [];
      service.messages$.subscribe((msg) => receivedMessages.push(msg));

      const messageHandler = mockWebview.addEventListener.calls
        .mostRecent()
        .args[1];

      messageHandler({ data: 0 });

      // 0 is a valid value and should be emitted
      expect(receivedMessages).toEqual([0]);
    });

    it('should emit message with empty string (valid primitive)', () => {
      service.connect();

      const receivedMessages: any[] = [];
      service.messages$.subscribe((msg) => receivedMessages.push(msg));

      const messageHandler = mockWebview.addEventListener.calls
        .mostRecent()
        .args[1];

      messageHandler({ data: '' });

      // Empty string is a valid value and should be emitted
      expect(receivedMessages).toEqual(['']);
    });

    it('should emit message with false boolean (valid primitive)', () => {
      service.connect();

      const receivedMessages: any[] = [];
      service.messages$.subscribe((msg) => receivedMessages.push(msg));

      const messageHandler = mockWebview.addEventListener.calls
        .mostRecent()
        .args[1];

      messageHandler({ data: false });

      // false is a valid value and should be emitted
      expect(receivedMessages).toEqual([false]);
    });
  });

  describe('Message Parsing and Serialization', () => {
    it('should parse JSON string with special characters', (done) => {
      service.connect();

      const specialData = {
        message: 'Test with "quotes" and \\backslashes\\',
        unicode: '🎉 Unicode test',
        newlines: 'Line1\nLine2\rLine3',
      };

      service.messages$.subscribe((message) => {
        expect(message).toEqual(specialData as any);
        done();
      });

      const messageHandler = mockWebview.addEventListener.calls
        .mostRecent()
        .args[1];
      messageHandler({ data: JSON.stringify(specialData) });
    });

    it('should handle messages with large nested structures', (done) => {
      service.connect();

      const deeplyNested: any = { level: 0 };
      let current = deeplyNested;
      for (let i = 1; i < 10; i++) {
        current.child = { level: i };
        current = current.child;
      }

      service.messages$.subscribe((message) => {
        expect(message).toEqual(deeplyNested as any);
        done();
      });

      const messageHandler = mockWebview.addEventListener.calls
        .mostRecent()
        .args[1];
      messageHandler({ data: deeplyNested });
    });

    it('should handle messages with arrays of various types', (done) => {
      service.connect();

      const arrayData = {
        numbers: [1, 2, 3, 4, 5],
        strings: ['a', 'b', 'c'],
        mixed: [1, 'two', true, null, { key: 'value' }],
        nested: [[1, 2], [3, 4], [5, 6]],
      };

      service.messages$.subscribe((message) => {
        expect(message).toEqual(arrayData as any);
        done();
      });

      const messageHandler = mockWebview.addEventListener.calls
        .mostRecent()
        .args[1];
      messageHandler({ data: arrayData });
    });

    it('should handle malformed JSON string gracefully', () => {
      service.connect();
      mockBus.push.calls.reset();

      const receivedMessages: any[] = [];
      let errorOccurred = false;

      service.messages$.subscribe({
        next: (msg) => receivedMessages.push(msg),
        error: () => { errorOccurred = true; }
      });

      const messageHandler = mockWebview.addEventListener.calls
        .mostRecent()
        .args[1];

      // Malformed JSON is pushed to LogBus and discarded — no throw, no stream error
      expect(() => {
        messageHandler({ data: '{invalid json}' });
      }).not.toThrow();

      expect(receivedMessages.length).toBe(0);
      expect(errorOccurred).toBe(false);
      expect(mockBus.push).toHaveBeenCalledWith(jasmine.objectContaining({
        level: 'warn',
        message: jasmine.stringContaining('failed to parse message'),
      }));
    });
  });

  describe('Connection Lifecycle', () => {
    it('should maintain connection state across multiple operations', () => {
      service.connect();
      
      service.send({ Action: 'Message1' } as any);
      service.send({ Action: 'Message2' } as any);
      service.send({ Action: 'Message3' } as any);

      expect(mockWebview.postMessage).toHaveBeenCalledTimes(3);
    });

    it('should allow reconnection after disconnect', () => {
      service.connect();
      expect(mockWebview.addEventListener).toHaveBeenCalledTimes(1);

      service.disconnect();
      expect(mockWebview.removeEventListener).toHaveBeenCalledTimes(1);

      service.connect();
      expect(mockWebview.addEventListener).toHaveBeenCalledTimes(2);
    });

    it('should handle rapid connect/disconnect cycles', () => {
      for (let i = 0; i < 10; i++) {
        service.connect();
        service.disconnect();
      }

      expect(mockWebview.addEventListener).toHaveBeenCalledTimes(10);
      expect(mockWebview.removeEventListener).toHaveBeenCalledTimes(10);
    });

    it('should not lose messages during active connection', (done) => {
      service.connect();

      const messageCount = 50;
      const receivedMessages: any[] = [];

      service.messages$.subscribe((msg) => {
        receivedMessages.push(msg);
        if (receivedMessages.length === messageCount) {
          expect(receivedMessages.length).toBe(messageCount);
          done();
        }
      });

      const messageHandler = mockWebview.addEventListener.calls
        .mostRecent()
        .args[1];

      for (let i = 0; i < messageCount; i++) {
        messageHandler({ data: { Action: `Message${i}`, Index: i } });
      }
    });
  });

  describe('Performance and Stress Tests', () => {
    it('should handle high-frequency message sending', () => {
      service.connect();

      for (let i = 0; i < 1000; i++) {
        service.send({ Action: `HighFreq${i}` } as any);
      }

      expect(mockWebview.postMessage).toHaveBeenCalledTimes(1000);
    });

    it('should handle large message payloads', () => {
      service.connect();

      const largeMessage: any = {
        Action: 'LargePayload',
        data: new Array(10000).fill('x').join(''),
      };

      service.send(largeMessage);

      expect(mockWebview.postMessage).toHaveBeenCalledWith(JSON.stringify(largeMessage));
    });

    it('should maintain message integrity under load', (done) => {
      service.connect();

      const messageCount = 500;
      const receivedMessages: any[] = [];

      service.messages$.subscribe((msg) => {
        receivedMessages.push(msg);
        if (receivedMessages.length === messageCount) {
          // Verify all messages received in order
          for (let i = 0; i < messageCount; i++) {
            expect((receivedMessages[i] as any).index).toBe(i);
          }
          done();
        }
      });

      const messageHandler = mockWebview.addEventListener.calls
        .mostRecent()
        .args[1];

      for (let i = 0; i < messageCount; i++) {
        messageHandler({ data: { Action: 'Stress', index: i } });
      }
    });
  });

  describe('Observable Behavior', () => {
    it('should emit to all subscribers simultaneously', () => {
      service.connect();

      const subscriber1Messages: any[] = [];
      const subscriber2Messages: any[] = [];
      const subscriber3Messages: any[] = [];

      service.messages$.subscribe((msg) => subscriber1Messages.push(msg));
      service.messages$.subscribe((msg) => subscriber2Messages.push(msg));
      service.messages$.subscribe((msg) => subscriber3Messages.push(msg));

      const testMessage = { Action: 'BroadcastTest' };
      const messageHandler = mockWebview.addEventListener.calls
        .mostRecent()
        .args[1];
      messageHandler({ data: testMessage });

      expect(subscriber1Messages).toEqual([testMessage]);
      expect(subscriber2Messages).toEqual([testMessage]);
      expect(subscriber3Messages).toEqual([testMessage]);
    });

    it('should not block on slow subscribers', (done) => {
      service.connect();

      let fastSubscriberCount = 0;
      let slowSubscriberCount = 0;

      service.messages$.subscribe(() => {
        fastSubscriberCount++;
      });

      service.messages$.subscribe(() => {
        // Simulate slow processing
        slowSubscriberCount++;
      });

      const messageHandler = mockWebview.addEventListener.calls
        .mostRecent()
        .args[1];

      messageHandler({ data: { Action: 'Message1' } });
      messageHandler({ data: { Action: 'Message2' } });

      setTimeout(() => {
        expect(fastSubscriberCount).toBe(2);
        expect(slowSubscriberCount).toBe(2);
        done();
      }, 10);
    });

    it('should support unsubscribing mid-stream', () => {
      service.connect();

      const receivedMessages: any[] = [];
      const subscription = service.messages$.subscribe((msg) => {
        receivedMessages.push(msg);
      });

      const messageHandler = mockWebview.addEventListener.calls
        .mostRecent()
        .args[1];

      messageHandler({ data: { Action: 'Message1' } });
      messageHandler({ data: { Action: 'Message2' } });

      subscription.unsubscribe();

      messageHandler({ data: { Action: 'Message3' } });

      expect(receivedMessages.length).toBe(2);
      expect((receivedMessages[0] as any).Action).toBe('Message1');
      expect((receivedMessages[1] as any).Action).toBe('Message2');
    });
  });

  describe('Error Recovery', () => {
    it('should push an error to LogBus on failed connection and allow reconnect after WebView2 becomes available', fakeAsync(() => {
      (window as any).chrome = undefined;

      // First connect attempt: WebView2 not available — stream stays open (no Subject.error)
      service.connect();
      tick(10000);
      expect(mockBus.push).toHaveBeenCalledWith(jasmine.objectContaining({
        level: 'error',
        message: jasmine.stringContaining('chrome.webview not available'),
      }));

      // Stream should still be open (no error emitted)
      let streamErrored = false;
      service.messages$.subscribe({ error: () => { streamErrored = true; } });
      expect(streamErrored).toBeFalse();

      // Restore WebView2 — a fresh service instance can connect successfully
      (window as any).chrome = mockChrome;
      const freshBus = jasmine.createSpyObj<LogBus>('LogBus', ['push']);
      TestBed.resetTestingModule();
      TestBed.configureTestingModule({
        providers: [
          ConnectionManager,
          { provide: LogBus, useValue: freshBus },
        ],
      });
      const freshService = TestBed.inject(ConnectionManager);
      expect(() => freshService.connect()).not.toThrow();
      expect(mockWebview.addEventListener).toHaveBeenCalled();
      freshService.disconnect();
    }));

    it('should handle disconnect on uninitialized service', () => {
      const freshBus = jasmine.createSpyObj<LogBus>('LogBus', ['push']);
      TestBed.resetTestingModule();
      TestBed.configureTestingModule({
        providers: [
          ConnectionManager,
          { provide: LogBus, useValue: freshBus },
        ],
      });
      const freshService = TestBed.inject(ConnectionManager);
      expect(() => freshService.disconnect()).not.toThrow();
    });

    it('should handle send on disconnected service', () => {
      service.connect();
      service.disconnect();

      // send() silently returns when not connected — no warn, no throw
      expect(() => service.send({ Action: 'AfterDisconnect' } as any)).not.toThrow();
      expect(mockWebview.postMessage).not.toHaveBeenCalled();
    });
  });

  describe('Memory Management', () => {
    it('should properly clean up on disconnect', () => {
      service.connect();
      const testMessage = { Action: 'TestCleanup' };

      service.messages$.subscribe(() => {});

      service.disconnect();

      expect(mockWebview.removeEventListener).toHaveBeenCalledWith(
        'message',
        jasmine.any(Function)
      );
    });

    it('should not accumulate event listeners on multiple connects', () => {
      for (let i = 0; i < 5; i++) {
        service.connect();
      }

      // Double-connect guard prevents re-registration: only 1 listener added
      expect(mockWebview.addEventListener).toHaveBeenCalledTimes(1);
    });
  });
});
