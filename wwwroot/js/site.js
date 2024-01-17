const FillPos = (lstDepartmentCtrl, lstPositionId) => {
    const lstPositions = $("#" + lstPositionId);
    lstPositions.empty();

    const selectedDepartment = lstDepartmentCtrl.options[lstDepartmentCtrl.selectedIndex].value;

    const positionSelect = document.getElementById(lstPositionId);

    if (selectedDepartment != null && selectedDepartment != '') {
        positionSelect.disabled = false;

        $.getJSON("http://136.228.167.31/AdminDepartment"+"/home/GetPositionsByDepartment", { DepartmentPkid: selectedDepartment }, (positions) => {
            if (positions != null && !jQuery.isEmptyObject(positions)) {
                positions.forEach((position) => {
                    lstPositions.append($('<option/>',
                        {
                            value: position.value,
                            text: position.text
                        }));
                });
            }
        });
    } else {
        positionSelect.disabled = true;
        lstPositions.empty();
    }
};

document.addEventListener("DOMContentLoaded", () => {
    const removeImageBtn = document.getElementById("removeImage");

    if (removeImageBtn) {
        removeImageBtn.style.display = "none";

        removeImageBtn.addEventListener("click", () => {
            const imagePreview = document.getElementById("imagePreview");
            const photoInput = document.getElementById("photoInput");

            imagePreview.setAttribute("src", "#");
            imagePreview.style.display = "none";
            removeImageBtn.style.display = "none";
            photoInput.value = "";
        });
    }

    const photoInput = document.getElementById("photoInput");

    if (photoInput) {
        photoInput.addEventListener("change", (e) => {
            const input = e.target;
            const imagePreview = document.getElementById("imagePreview");
            const removeImageBtn = document.getElementById("removeImage");
            const photoError = document.getElementById("photoError");

            if (input.files && input.files[0]) {
                imagePreview.setAttribute(
                    "src",
                    URL.createObjectURL(input.files[0])
                );
                imagePreview.style.display = "block";
                removeImageBtn.style.display = "block";
                photoError.innerHTML = "";
            }
        });
    }
});


let card = document.querySelector(".card"); //declearing profile card element
let displayPicture = document.querySelector(".display-picture"); //declearing profile picture

displayPicture.addEventListener("click", function () { //on click on profile picture toggle hidden class from css
    card.classList.toggle("hidden")
})

//StaffID show Form
function showForm() {
    var overlay = document.getElementById('overlay');
    var form = document.getElementById('staffForm');

    overlay.style.display = 'block';
    form.style.display = 'block';
}

// Optional: Close the form and overlay when clicking outside the form
document.getElementById('overlay').addEventListener('click', function (event) {
    if (event.target === this) {
        this.style.display = 'none';
        document.getElementById('staffForm').style.display = 'none';
    }
});

