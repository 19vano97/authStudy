document.getElementById("login-form").addEventListener("submit", async function (e) {
    e.preventDefault();

    const form = e.target;
    const formData = new FormData(form);
    const response = await fetch(form.action, {
        method: "POST",
        body: formData,
    });

    const html = await response.text();
    const parser = new DOMParser();
    const doc = parser.parseFromString(html, "text/html");
    const newForm = doc.querySelector("#login-form");

    if (newForm) {
        form.outerHTML = newForm.outerHTML;
    } else {
        window.location.href = response.url;
    }
});