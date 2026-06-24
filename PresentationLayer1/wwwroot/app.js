// Convert UTC datetime strings to browser local time
document.querySelectorAll('time.local-time').forEach(el => {
  const d = new Date(el.dateTime);
  if (!isNaN(d)) {
    el.textContent = d.toLocaleString(undefined, { dateStyle: 'medium', timeStyle: 'short' });
  }
});

// Copy-link buttons
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

// Mobile nav toggle
const navToggle = document.querySelector('.nav-toggle');
const nav = document.querySelector('.site-header nav');
if (navToggle && nav) {
  navToggle.addEventListener('click', () => {
    const open = nav.classList.toggle('open');
    navToggle.setAttribute('aria-expanded', open);
    navToggle.textContent = open ? '✕' : '☰';
  });
}

// Toast notifications from TempData notices
function showToast(message, type = 'success') {
  let container = document.querySelector('.toast-container');
  if (!container) {
    container = document.createElement('div');
    container.className = 'toast-container';
    document.body.appendChild(container);
  }
  const toast = document.createElement('div');
  toast.className = `toast ${type}`;
  toast.textContent = message;
  container.appendChild(toast);
  setTimeout(() => {
    toast.style.opacity = '0';
    toast.style.transform = 'translateX(24px)';
    toast.style.transition = 'opacity 0.3s, transform 0.3s';
    setTimeout(() => toast.remove(), 300);
  }, 4000);
}

document.querySelectorAll('.notice').forEach(notice => {
  const type = notice.classList.contains('error') ? 'error' : 'success';
  showToast(notice.textContent.trim(), type);
  notice.style.display = 'none';
});

// Event search & filter
const searchInput = document.querySelector('#event-search');
const filterPills = document.querySelectorAll('.filter-pill');
const eventCards = document.querySelectorAll('.js-event-card');

function filterEvents() {
  const query = (searchInput?.value ?? '').toLowerCase().trim();
  const activeFilter = document.querySelector('.filter-pill.active')?.dataset.filter ?? 'all';

  eventCards.forEach(card => {
    const title = card.dataset.title?.toLowerCase() ?? '';
    const desc = card.dataset.description?.toLowerCase() ?? '';
    const status = card.dataset.status ?? '';
    const isFull = card.dataset.full === 'true';

    const matchesSearch = !query || title.includes(query) || desc.includes(query);
    let matchesFilter = true;
    if (activeFilter === 'available') matchesFilter = !isFull;
    else if (activeFilter === 'full') matchesFilter = isFull;
    else if (activeFilter !== 'all') matchesFilter = status === activeFilter;

    card.style.display = matchesSearch && matchesFilter ? '' : 'none';
  });

  const visible = [...eventCards].filter(c => c.style.display !== 'none').length;
  const emptyMsg = document.querySelector('.js-filter-empty');
  if (emptyMsg) emptyMsg.hidden = visible > 0;
}

if (searchInput) {
  searchInput.addEventListener('input', filterEvents);
}

filterPills.forEach(pill => {
  pill.addEventListener('click', () => {
    filterPills.forEach(p => p.classList.remove('active'));
    pill.classList.add('active');
    filterEvents();
  });
});

// Scroll reveal
const revealEls = document.querySelectorAll('.reveal');
if (revealEls.length) {
  const observer = new IntersectionObserver(entries => {
    entries.forEach(entry => {
      if (entry.isIntersecting) {
        entry.target.classList.add('visible');
        observer.unobserve(entry.target);
      }
    });
  }, { threshold: 0.1 });
  revealEls.forEach(el => observer.observe(el));
}

// Stagger card animations
document.querySelectorAll('.js-event-card').forEach((card, i) => {
  card.style.animationDelay = `${i * 0.07}s`;
});
