// Claim amount calculation
function calculateClaimAmount(moduleId, hours) {
    if (moduleId && hours) {
        fetch(`/Claims/CalculateAmount?moduleId=${moduleId}&hours=${hours}`)
            .then(response => response.json())
            .then(data => {
                document.getElementById('calculatedAmount').value =
                    new Intl.NumberFormat('en-US', { style: 'currency', currency: 'USD' })
                        .format(data.amount);
            });
    }
}

// Real-time validation
function validateClaim(moduleId, hours) {
    if (moduleId && hours) {
        fetch(`/Claims/Validate?moduleId=${moduleId}&hours=${hours}`)
            .then(response => response.json())
            .then(data => {
                const container = document.getElementById('validationMessages');
                container.innerHTML = '';

                data.messages.forEach(message => {
                    const div = document.createElement('div');
                    div.className = `validation-message ${message.severity.toLowerCase()}`;
                    div.textContent = message.text;
                    container.appendChild(div);
                });
            });
    }
}

// Document upload validation
function validateDocument(input) {
    const file = input.files[0];
    if (file) {
        if (file.type !== 'application/pdf') {
            alert('Please upload PDF files only');
            input.value = '';
            return false;
        }
        if (file.size > 5242880) { // 5MB
            alert('File size should not exceed 5MB');
            input.value = '';
            return false;
        }
    }
    return true;
}

// Initialize tooltips and popovers
document.addEventListener('DOMContentLoaded', function () {
    const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });

    const popoverTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="popover"]'));
    popoverTriggerList.map(function (popoverTriggerEl) {
        return new bootstrap.Popover(popoverTriggerEl);
    });
});

// Auto-dismiss alerts
window.setTimeout(function () {
    const alerts = document.querySelectorAll('.alert:not(.alert-permanent)');
    alerts.forEach(alert => {
        const bsAlert = new bootstrap.Alert(alert);
        bsAlert.close();
    });
}, 5000);
