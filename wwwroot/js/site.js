(function () {
    'use strict';

    const shell = document.getElementById('appShell');
    const sidebar = document.getElementById('appSidebar');
    const overlay = document.getElementById('sidebarOverlay');
    const collapseBtn = document.getElementById('sidebarCollapseBtn');
    const mobileToggle = document.getElementById('sidebarToggleMobile');
    const storageKey = 'biogama-sidebar-collapsed';

    if (shell && localStorage.getItem(storageKey) === 'true') {
        shell.classList.add('sidebar-collapsed');
    }

    if (collapseBtn) {
        collapseBtn.addEventListener('click', function () {
            shell.classList.toggle('sidebar-collapsed');
            localStorage.setItem(storageKey, shell.classList.contains('sidebar-collapsed'));
        });
    }

    if (mobileToggle) {
        mobileToggle.addEventListener('click', function () {
            shell.classList.toggle('sidebar-open');
        });
    }

    if (overlay) {
        overlay.addEventListener('click', function () {
            shell.classList.remove('sidebar-open');
        });
    }

    document.querySelectorAll('.sidebar-link').forEach(function (link) {
        link.addEventListener('click', function () {
            if (window.innerWidth < 992) {
                shell.classList.remove('sidebar-open');
            }
        });
    });

    const tooltipTriggerList = document.querySelectorAll('[data-bs-toggle="tooltip"]');
    tooltipTriggerList.forEach(function (el) {
        new bootstrap.Tooltip(el);
    });
})();
