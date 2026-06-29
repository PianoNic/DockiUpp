import { defineConfig } from '@playwright/test';

// Note: the backend API must already be running (it needs Postgres + the Docker socket);
// this config only boots the Angular dev server. Use http://localhost (not 127.0.0.1) so the
// browser origin matches the API's CORS allow-list.
export default defineConfig({
  testDir: '.',
  timeout: 30000,
  reporter: [['list']],
  use: {
    baseURL: 'http://localhost:4200',
    headless: true,
    screenshot: 'only-on-failure',
    // PLAYWRIGHT_BROWSERS_PATH points at the pre-installed Chromium; override the binary directly
    // when the bundled revision differs from this Playwright version's expectation.
    launchOptions: process.env.PW_CHROMIUM_PATH
      ? { executablePath: process.env.PW_CHROMIUM_PATH }
      : {},
  },
  webServer: {
    command: 'npm start -- --port 4200 --host localhost',
    url: 'http://localhost:4200',
    reuseExistingServer: true,
    timeout: 120000,
  },
});
