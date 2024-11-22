const ClaimCalculator = {
    init() {
        this.moduleSelect = document.getElementById('moduleSelect');
        this.hoursInput = document.getElementById('hoursWorked');
        this.amountDisplay = document.getElementById('calculatedAmount');
        if (this.moduleSelect && this.hoursInput) {
            this.bindEvents();
        }
    },
    bindEvents() {
        this.moduleSelect.addEventListener('change', () => this.calculateAmount());
        this.hoursInput.addEventListener('input', () => this.calculateAmount());
    },
    calculateAmount() {
        const moduleId = this.moduleSelect.value;
        const hours = this.hoursInput.value;
        if (moduleId && hours > 0) {
            const hourlyRate = this.moduleSelect.selectedOptions[0].dataset.rate;
            const amount = hourlyRate * hours;
            this.amountDisplay.value = new Intl.NumberFormat('en-ZA', {
                style: 'currency',
                currency: 'ZAR'
            }).format(amount);
        }
    }
};

document.addEventListener('DOMContentLoaded', () => ClaimCalculator.init());
