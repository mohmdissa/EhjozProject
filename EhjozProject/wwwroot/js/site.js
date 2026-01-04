// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Site-wide confirmation modal (replaces browser confirm()).
(() => {
  function initConfirmModal() {
    const modalEl = document.getElementById("confirmModal");
    if (!modalEl || !window.bootstrap) return;

    const modal = new bootstrap.Modal(modalEl);
    const titleEl = document.getElementById("confirmModalTitle");
    const bodyEl = document.getElementById("confirmModalBody");
    const confirmBtn = document.getElementById("confirmModalConfirmBtn");

    let pendingForm = null;

    document.addEventListener("submit", (e) => {
      const form = e.target;
      if (!(form instanceof HTMLFormElement)) return;

      const msg = form.getAttribute("data-confirm");
      if (!msg) return;

      // Allow real submit after user confirmed
      if (form.dataset.confirming === "1") return;

      e.preventDefault();

      pendingForm = form;
      if (titleEl) titleEl.textContent = form.getAttribute("data-confirm-title") || "Please confirm";
      if (bodyEl) bodyEl.textContent = msg;

      const btnText = form.getAttribute("data-confirm-btn") || "Confirm";
      if (confirmBtn) confirmBtn.textContent = btnText;

      modal.show();
    });

    confirmBtn?.addEventListener("click", () => {
      if (!pendingForm) return;
      pendingForm.dataset.confirming = "1";
      pendingForm.submit();
      pendingForm = null;
      modal.hide();
    });
  }

  if (document.readyState === "loading") {
    document.addEventListener("DOMContentLoaded", initConfirmModal);
  } else {
    initConfirmModal();
  }
})();
