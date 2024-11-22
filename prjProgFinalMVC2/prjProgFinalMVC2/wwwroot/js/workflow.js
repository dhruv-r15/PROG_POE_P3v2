const WorkflowManager = {
    init() {
        this.bindEvents();
        this.initializeTooltips();
    },

    bindEvents() {
        document.addEventListener('DOMContentLoaded', () => {
            const workflowButtons = document.querySelectorAll('.workflow-actions button');
            workflowButtons.forEach(button => {
                button.addEventListener('click', this.handleWorkflowAction.bind(this));
            });
        });
    },

    initializeTooltips() {
        const tooltips = document.querySelectorAll('[data-bs-toggle="tooltip"]');
        tooltips.forEach(tooltip => {
            new bootstrap.Tooltip(tooltip);
        });
    },

    async processClaim(claimId, action) {
        try {
            const response = await fetch('/Claims/ProcessClaim', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
                },
                body: JSON.stringify({ id: claimId, action: action })
            });

            const result = await response.json();
            if (result.success) {
                this.updateWorkflowUI(claimId, action);
                this.showNotification('Success', `Claim ${action.toLowerCase()} successfully`, 'success');
            } else {
                this.showNotification('Error', result.message, 'error');
            }
        } catch (error) {
            this.showNotification('Error', 'An error occurred while processing the claim', 'error');
        }
    },

    updateWorkflowUI(claimId, action) {
        const workflowStep = document.querySelector(`[data-claim-id="${claimId}"]`);
        if (workflowStep) {
            const steps = workflowStep.querySelectorAll('.workflow-step');
            steps.forEach(step => {
                step.classList.remove('current');
                if (step.dataset.step === action.toLowerCase()) {
                    step.classList.add('completed');
                }
            });
        }
    },

    showNotification(title, message, type) {
        const toast = document.createElement('div');
        toast.className = `toast align-items-center text-white bg-${type === 'success' ? 'success' : 'danger'}`;
        toast.setAttribute('role', 'alert');
        toast.innerHTML = `
            <div class="d-flex">
                <div class="toast-body">
                    <strong>${title}:</strong> ${message}
                </div>
                <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast"></button>
            </div>
        `;

        const container = document.getElementById('toast-container') || document.body;
        container.appendChild(toast);

        const bsToast = new bootstrap.Toast(toast);
        bsToast.show();

        toast.addEventListener('hidden.bs.toast', () => {
            toast.remove();
        });
    }
};

WorkflowManager.init();
