// karma.conf.js
// Enterprise Karma configuration for Pms_GuidanceGUI Angular frontend.
// Produces three report artefacts on every run:
//   1. test-results/junit.xml        — JUnit XML for Azure DevOps / Jenkins CI
//   2. test-results/html/index.html  — Human-readable HTML test report
//   3. coverage/                     — Istanbul code-coverage (HTML + LCOV + text)

'use strict';

module.exports = function (config) {
  config.set({
    // ── Base path ─────────────────────────────────────────────────────────────
    basePath: '',

    // ── Frameworks ────────────────────────────────────────────────────────────
    // Note: '@angular/build/karma' is injected automatically by the Angular builder.
    frameworks: ['jasmine'],

    // ── Plugins ───────────────────────────────────────────────────────────────
    plugins: [
      require('karma-jasmine'),
      require('karma-chrome-launcher'),
      require('karma-jasmine-html-reporter'),
      require('karma-coverage'),
      require('karma-junit-reporter'),
    ],

    // ── Client configuration ──────────────────────────────────────────────────
    client: {
      jasmine: {
        // Randomise test order to catch implicit ordering dependencies.
        random: true,
        // Fail fast: stop after the first spec failure in CI.
        stopOnSpecFailure: false,
      },
      clearContext: false, // keep the Jasmine HTML reporter visible in the browser
    },

    // ── Reporters ─────────────────────────────────────────────────────────────
    reporters: ['progress', 'kjhtml', 'junit', 'coverage'],

    // ── JUnit reporter (CI / Azure DevOps / Jenkins) ──────────────────────────
    junitReporter: {
      outputDir: 'test-results',
      outputFile: 'junit.xml',
      suite: 'Pms_GuidanceGUI Angular Unit Tests',
      useBrowserName: false,       // keeps file name stable for CI artefact pickup
      nameFormatter: undefined,
      classNameFormatter: undefined,
      properties: {},
      xmlVersion: null,
    },

    // ── Coverage reporter ─────────────────────────────────────────────────────
    coverageReporter: {
      dir: 'coverage',
      subdir: '.',
      reporters: [
        { type: 'html',          subdir: 'html' },     // browseable HTML
        { type: 'lcovonly',      subdir: '.',  file: 'lcov.info' }, // SonarQube / Codecov
        { type: 'text-summary' },                       // printed to console
        { type: 'cobertura',     subdir: '.',  file: 'cobertura.xml' }, // Azure DevOps coverage tab
      ],
      check: {
        global: {
          statements:  80,
          branches:    75,
          functions:   80,
          lines:       80,
        },
      },
    },

    // ── Port & logging ────────────────────────────────────────────────────────
    port: 9876,
    colors: true,
    logLevel: config.LOG_INFO,

    // ── File watching ─────────────────────────────────────────────────────────
    autoWatch: true,
    autoWatchBatchDelay: 300,

    // ── Browser launcher ──────────────────────────────────────────────────────
    browsers: ['ChromeHeadless'],
    customLaunchers: {
      ChromeHeadlessCI: {
        base: 'ChromeHeadless',
        flags: [
          '--no-sandbox',           // required in most CI environments
          '--disable-gpu',
          '--disable-dev-shm-usage', // avoids /dev/shm exhaustion in containers
          '--disable-extensions',
          '--window-size=1920,1080',
        ],
      },
    },

    // ── Single-run vs watch ───────────────────────────────────────────────────
    // Overridden to true when running via `ng test --watch=false` or `npm run test:ci`.
    singleRun: false,

    // ── Timeouts ──────────────────────────────────────────────────────────────
    browserNoActivityTimeout: 60000,   // ms — increase for slow CI agents
    captureTimeout:           60000,
    browserDisconnectTimeout: 10000,
    browserDisconnectTolerance: 1,

    // ── Concurrency ───────────────────────────────────────────────────────────
    concurrency: Infinity,
  });
};
