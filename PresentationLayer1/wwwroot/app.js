// Convert UTC datetime strings to browser local time
document.querySelectorAll('time.local-time').forEach(el => {
  const d = new Date(el.dateTime);
  if (!isNaN(d)) {
    el.textContent = d.toLocaleString(undefined, { dateStyle: 'medium', timeStyle: 'short' });
  }
});

// Copy-link buttons: data-url for a specific URL, or copies current page URL
document.querySelectorAll('.js-copy-link').forEach(btn => {
  btn.addEventListener('click', async () => {
    const url = btn.dataset.url
      ? new URL(btn.dataset.url, window.location.origin).href
      : window.location.href;
    try {
      await navigator.clipboard.writeText(url);
      const orig = btn.textContent;
      btn.textContent = 'Copied!';
      setTimeout(() => { btn.textContent = orig; }, 1500);
    } catch { /* clipboard unavailable */ }
  });
});
