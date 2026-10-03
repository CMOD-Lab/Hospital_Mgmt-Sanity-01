// Clinic Management System - Site JavaScript

// Auto-dismiss alerts after 5 seconds
document.addEventListener('DOMContentLoaded', function () {
    const alerts = document.querySelectorAll('.alert.alert-dismissible');
    alerts.forEach(function (alert) {
        setTimeout(function () {
            const bsAlert = bootstrap.Alert.getOrCreateInstance(alert);
            if (bsAlert) bsAlert.close();
        }, 5000);
    });
});

// Confirm delete actions
document.querySelectorAll('[data-confirm]').forEach(function (element) {
    element.addEventListener('click', function (e) {
        if (!confirm(this.dataset.confirm)) {
            e.preventDefault();
        }
    });
});
