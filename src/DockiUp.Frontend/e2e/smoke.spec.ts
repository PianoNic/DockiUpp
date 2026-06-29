import { test, expect } from '@playwright/test';

test('app loads, renders the DockiUp shell, and talks to the live API', async ({ page }) => {
  const consoleErrors: string[] = [];
  page.on('console', (msg) => {
    if (msg.type() === 'error') consoleErrors.push(msg.text());
  });

  // Watch the real API call the app makes on load (projects list).
  const projectsResponse = page.waitForResponse(
    (r) => r.url().includes('/api/') && r.request().method() === 'GET',
    { timeout: 15000 },
  );

  await page.goto('/', { waitUntil: 'networkidle' });

  // App shell from PianUI: the header shows the DockiUp brand.
  await expect(page.locator('.app-title')).toHaveText('DockiUp');

  // Sidenav navigation items render.
  await expect(page.getByRole('button', { name: /Dashboard/i })).toBeVisible();
  await expect(page.getByRole('button', { name: /Containers/i })).toBeVisible();

  // The app actually reached the backend.
  const resp = await projectsResponse;
  expect(resp.status(), `API ${resp.url()} returned ${resp.status()}`).toBeLessThan(500);

  await page.screenshot({ path: 'dockiup-smoke.png', fullPage: true });

  // Fail on real app errors (ignore noisy favicon/font 404s).
  const realErrors = consoleErrors.filter(
    (e) => !/favicon|fonts\.googleapis|net::ERR_/i.test(e),
  );
  expect(realErrors, `console errors:\n${realErrors.join('\n')}`).toHaveLength(0);
});
