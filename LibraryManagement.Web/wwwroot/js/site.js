(() => {
    const alerts = document.querySelectorAll('.alert-dismissible');
    alerts.forEach(alert => {
        setTimeout(() => {
            const closeButton = alert.querySelector('[data-bs-dismiss="alert"]');
            if (closeButton) {
                closeButton.click();
            }
        }, 5000);
    });
})();
