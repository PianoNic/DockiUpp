import { test, expect } from '@playwright/test';
import { readFileSync } from 'node:fs';

const containerId = readFileSync(
  '/tmp/claude-0/-home-user/b5726dc7-0bce-54bf-9199-f3ae9d3d1224/scratchpad/cid.txt',
  'utf8',
).trim();

test('web terminal opens a shell into a container and runs a command', async ({ page }) => {
  await page.goto(`/terminal/${containerId}`, { waitUntil: 'domcontentloaded' });

  // StartExec round-tripped through the live backend + docker -> status becomes "open".
  await expect(page.locator('.status')).toHaveText('open', { timeout: 20000 });

  // Type a command into xterm and verify the container echoes the (shell-evaluated) result back.
  await page.locator('.xterm-screen').click();
  await page.keyboard.type('echo pw-terminal-ok-$((3*7))');
  await page.keyboard.press('Enter');

  await expect(page.locator('.xterm-rows')).toContainText('pw-terminal-ok-21', { timeout: 10000 });

  await page.screenshot({ path: 'dockiup-terminal.png' });
});
