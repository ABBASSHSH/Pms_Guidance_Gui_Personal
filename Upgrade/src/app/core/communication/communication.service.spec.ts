import { TestBed } from '@angular/core/testing';
import { CommunicationService } from './communication.service';
import { ConnectionManager } from './connection.manager';
import { Subject } from 'rxjs';
import { RawMessage } from './raw-message';

describe('CommunicationService', () => {
  let service: CommunicationService;
  let mockConnectionManager: jasmine.SpyObj<ConnectionManager>;
  let messageSubject: Subject<RawMessage>;

  beforeEach(() => {
    messageSubject = new Subject<RawMessage>();

    mockConnectionManager = jasmine.createSpyObj('ConnectionManager', [
      'connect',
      'send',
      'disconnect',
    ], {
      messages$: messageSubject.asObservable(),
    });

    TestBed.configureTestingModule({
      providers: [
        CommunicationService,
        { provide: ConnectionManager, useValue: mockConnectionManager },
      ],
    });

    service = TestBed.inject(CommunicationService);
  });

  describe('Initialization', () => {
    it('should create the service', () => {
      expect(service).toBeTruthy();
    });

    it('should connect successfully', () => {
      service.connect();
      expect(mockConnectionManager.connect).toHaveBeenCalledTimes(1);
    });
  });

  describe('Message Sending', () => {
    it('should send action-based message without payload', () => {
      const action = 'TestAction';
      service.send(action);

      expect(mockConnectionManager.send).toHaveBeenCalledWith({
        CallContext: { Action: action },
        Payload: {},
      } as any);
    });

    it('should send action-based message with payload', () => {
      const action = 'TestAction';
      const payload = { key1: 'value1', key2: 123 };

      service.send(action, payload);

      expect(mockConnectionManager.send).toHaveBeenCalledWith({
        CallContext: { Action: action },
        Payload: { key1: 'value1', key2: 123 },
      } as any);
    });

    it('should handle empty payload object', () => {
      const action = 'TestAction';
      service.send(action, {});

      expect(mockConnectionManager.send).toHaveBeenCalledWith({
        CallContext: { Action: action },
        Payload: {},
      } as any);
    });

    it('should handle complex payload structures', () => {
      const action = 'ComplexAction';
      const payload = {
        nested: { data: { value: 'test' } },
        array: [1, 2, 3],
        boolean: true,
      };

      service.send(action, payload);

      expect(mockConnectionManager.send).toHaveBeenCalledWith({
        CallContext: { Action: action },
        Payload: {
          nested: { data: { value: 'test' } },
          array: [1, 2, 3],
          boolean: true,
        },
      } as any);
    });
  });

  describe('Shutdown', () => {
    it('should disconnect from connection manager', () => {
      service.shutdown();
      expect(mockConnectionManager.disconnect).toHaveBeenCalledTimes(1);
    });

    it('should be safe to call shutdown multiple times', () => {
      service.shutdown();
      service.shutdown();
      expect(mockConnectionManager.disconnect).toHaveBeenCalledTimes(2);
    });
  });

  describe('Integration Scenarios', () => {
    it('should handle full lifecycle: connect, send, shutdown', () => {
      service.connect();
      service.send('TestAction', { data: 'test' });

      expect(mockConnectionManager.send).toHaveBeenCalled();

      service.shutdown();
      expect(mockConnectionManager.disconnect).toHaveBeenCalled();
    });

    it('should handle send before connect gracefully', () => {
      expect(() => service.send('TestAction')).not.toThrow();
      expect(mockConnectionManager.send).toHaveBeenCalled();
    });

    it('should handle rapid successive sends', () => {
      for (let i = 0; i < 100; i++) {
        service.send(`Action${i}`, { index: i });
      }
      expect(mockConnectionManager.send).toHaveBeenCalledTimes(100);
    });
  });

  describe('Edge Cases and Error Scenarios', () => {
    it('should handle null action in send', () => {
      expect(() => service.send(null as any)).not.toThrow();
      expect(mockConnectionManager.send).toHaveBeenCalledWith({
        CallContext: { Action: null },
        Payload: {},
      } as any);
    });

    it('should handle undefined action in send', () => {
      expect(() => service.send(undefined as any)).not.toThrow();
      expect(mockConnectionManager.send).toHaveBeenCalledWith({
        CallContext: { Action: undefined },
        Payload: {},
      } as any);
    });

    it('should handle empty string action', () => {
      service.send('');
      expect(mockConnectionManager.send).toHaveBeenCalledWith({
        CallContext: { Action: '' },
        Payload: {},
      } as any);
    });

    it('should handle payload with null values', () => {
      service.send('TestAction', { key: null, value: undefined });
      expect(mockConnectionManager.send).toHaveBeenCalledWith({
        CallContext: { Action: 'TestAction' },
        Payload: { key: null, value: undefined },
      } as any);
    });

    it('should handle payload with circular references gracefully', () => {
      const circular: any = { Action: 'Test' };
      circular.self = circular;

      expect(() => service.send('CircularTest', circular)).not.toThrow();
    });

    it('should handle very large payloads', () => {
      const largePayload: Record<string, any> = {};
      for (let i = 0; i < 1000; i++) {
        largePayload[`key${i}`] = `value${i}`;
      }

      service.send('LargeAction', largePayload);
      expect(mockConnectionManager.send).toHaveBeenCalled();
    });

    it('should handle special characters in action names', () => {
      const specialActions = [
        'Action-With-Dashes',
        'Action_With_Underscores',
        'Action.With.Dots',
        'Action@With#Special$Chars',
      ];

      specialActions.forEach(action => {
        service.send(action);
        expect(mockConnectionManager.send).toHaveBeenCalledWith({
          CallContext: { Action: action },
          Payload: {},
        } as any);
      });
    });

    it('should handle payload with arrays', () => {
      service.send('ArrayAction', {
        numbers: [1, 2, 3],
        strings: ['a', 'b', 'c'],
        mixed: [1, 'two', true, null],
      });

      expect(mockConnectionManager.send).toHaveBeenCalled();
    });

    it('should handle payload with dates', () => {
      const testDate = new Date('2026-02-06');
      service.send('DateAction', { timestamp: testDate });

      expect(mockConnectionManager.send).toHaveBeenCalled();
    });

    it('should handle multiple connect attempts', () => {
      service.connect();
      service.connect();
      service.connect();

      expect(mockConnectionManager.connect).toHaveBeenCalledTimes(3);
    });

    it('should handle connect after shutdown', () => {
      service.connect();
      service.shutdown();
      service.connect();

      expect(mockConnectionManager.connect).toHaveBeenCalledTimes(2);
      expect(mockConnectionManager.disconnect).toHaveBeenCalledTimes(1);
    });
  });

  describe('Type Safety and Validation', () => {
    it('should handle messages with different types', () => {
      const testMessages = [
        { Action: 'StringAction', data: 'string' },
        { Action: 'NumberAction', data: 123 },
        { Action: 'BooleanAction', data: true },
        { Action: 'ObjectAction', data: { nested: 'object' } },
        { Action: 'ArrayAction', data: [1, 2, 3] },
      ];

      testMessages.forEach(msg => {
        service.send(msg.Action, { data: msg.data });
        expect(mockConnectionManager.send).toHaveBeenCalledWith({
          CallContext: { Action: msg.Action },
          Payload: { data: msg.data },
        } as any);
      });
    });

    it('should preserve payload property types', () => {
      const payload = {
        string: 'test',
        number: 42,
        boolean: true,
        nullValue: null,
        undefinedValue: undefined,
        object: { nested: 'value' },
        array: [1, 2, 3],
      };

      service.send('TypeTest', payload);

      const call = mockConnectionManager.send.calls.mostRecent();
      const sentMessage = call.args[0] as any;

      expect(sentMessage).toEqual({
        CallContext: { Action: 'TypeTest' },
        Payload: payload,
      } as any);
    });
  });
});
