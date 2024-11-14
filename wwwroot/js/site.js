const FillPos = (lstDepartmentCtrl, lstPositionId) => {
    const lstPositions = $("#" + lstPositionId);
    lstPositions.empty();

    const selectedDepartment = lstDepartmentCtrl.options[lstDepartmentCtrl.selectedIndex].value;

    const positionSelect = document.getElementById(lstPositionId);

    if (selectedDepartment != null && selectedDepartment != '') {
        positionSelect.disabled = false;

        $.getJSON("http://localhost:7655" + "/home/GetPositionsByDepartment", { DepartmentPkid: selectedDepartment }, (positions) => {
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

const FillInitialPos = (lstDepartmentCtrl, lstInitialPositionId) => {
    const lstInitialPositions = $("#" + lstInitialPositionId);
    lstInitialPositions.empty();

    const selectedDepartment = lstDepartmentCtrl.options[lstDepartmentCtrl.selectedIndex].value;

    const initialPositionSelect = document.getElementById(lstInitialPositionId);

    if (selectedDepartment != null && selectedDepartment != '') {
        initialPositionSelect.disabled = false;

        $.getJSON("http://localhost:7655" + "/home/GetInitialPositionsByDepartment", { DepartmentPkid: selectedDepartment }, (positions) => {
            if (positions != null && !jQuery.isEmptyObject(positions)) {
                positions.forEach((position) => {
                    lstInitialPositions.append($('<option/>',
                        {
                            value: position.value,
                            text: position.text
                        }));
                });
            }
        });
    } else {
        initialPositionSelect.disabled = true;
        lstInitialPositions.empty();
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


let card = document.querySelector(".card");
let displayPicture = document.querySelector(".display-picture");

displayPicture.addEventListener("click", function () {
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

//Go to certain route when user click search/excel download
$(document).ready(function () {
    $("#btnSearchStaff").click(function () {
        $("#ListForm").attr("action", "/Home/SearchStaffs");
        $("#ListForm").submit();
    });

    $("#btnExportToExcel").click(function () {
        var searchTerm = $("#SearchTerm").val();

        if (searchTerm && searchTerm.trim() !== "") {
            $("#ListForm").attr("action", "/Home/ExcelExportSearchResult");
        } else {
            $("#ListForm").attr("action", "/Home/ExcelAllStaffExport");
        }

        $("#ListForm").submit();
    });

});