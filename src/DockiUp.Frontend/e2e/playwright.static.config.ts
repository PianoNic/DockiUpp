import { defineConfig } from '@playwright/test';

// Runs against the control-plane API serving the pre-built SPA (same origin, :5098). No dev server —
// the API hosts wwwroot — so the suite starts instantly. The API + a node must already be running.
export default defineConfig({
  testDir: '.',
  timeout: 30000,
  reporter: [['list']],
  use: {
    baseURL: 'http://localhost:5098',
    headless: true,
    screenshot: 'only-on-failure',
    ignoreHTTPSErrors: true,
    launchOptions: process.env.PW_CHROMIUM_PATH
      ? { executablePath: process.env.PW_CHROMIUM_PATH }
      : {},
  },
});
