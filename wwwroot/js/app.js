// =============================================================
// Sınav Yönetim Sistemi - UI Etkileşimleri
// =============================================================

(function () {
    'use strict';

    // ---- Mobile menu toggle ----
    function initMobileMenu() {
        const btn = document.getElementById('mobileMenuBtn');
        const menu = document.getElementById('mobileMenu');
        if (!btn || !menu) return;

        btn.addEventListener('click', function () {
            const isOpen = menu.classList.toggle('hidden') === false;
            btn.setAttribute('aria-expanded', isOpen ? 'true' : 'false');
        });
    }

    // ---- Animated counters ----
    function initCounters() {
        const els = document.querySelectorAll('[data-count]');
        els.forEach(function (el) {
            const target = parseInt(el.getAttribute('data-count'), 10) || 0;
            if (target === 0) { el.textContent = '0'; return; }
            const duration = 900;
            const start = performance.now();
            function step(now) {
                const p = Math.min((now - start) / duration, 1);
                const eased = 1 - Math.pow(1 - p, 3);
                el.textContent = Math.round(target * eased).toString();
                if (p < 1) requestAnimationFrame(step);
            }
            requestAnimationFrame(step);
        });
    }

    // ---- Auto-hide alerts ----
    function initAlerts() {
        document.querySelectorAll('[data-auto-dismiss]').forEach(function (el) {
            setTimeout(function () {
                el.style.transition = 'opacity .4s ease';
                el.style.opacity = '0';
                setTimeout(function () { el.remove(); }, 400);
            }, 4000);
        });
    }

    // ---- Stagger entrance animation ----
    function initStagger() {
        document.querySelectorAll('[data-stagger]').forEach(function (container) {
            Array.from(container.children).forEach(function (child, i) {
                child.classList.add('animate-in');
                child.style.animationDelay = (i * 0.06) + 's';
            });
        });
    }

    document.addEventListener('DOMContentLoaded', function () {
        initMobileMenu();
        initCounters();
        initAlerts();
        initStagger();
    });

    // Expose helpers
    window.AppUI = {
        refreshCounters: initCounters
    };
})();
