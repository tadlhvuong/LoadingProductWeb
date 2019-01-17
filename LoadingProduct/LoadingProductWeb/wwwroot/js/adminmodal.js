$(document).ready(function () {
	$("form input:checkbox").on('change', function () {
		if ($(this).is(':checked')) {
			$(this).attr('value', 'true');
		} else {
			$(this).attr('value', 'false');
		}
	});
	$("a[data-modal]").on("click", function (e) {
		e.preventDefault();
		$("#modalContent").load(this.href, function () {
			$("#modalContainer").modal({ keyboard: true }, "show");
		});
	});
	$(document).on("click", ".modal-links", function (e) {
		e.preventDefault();
		$("#modalContent").load(this.href);
	});
	$(document).on("click", ".modal-closer", function (e) {
		e.preventDefault();
		$("#modalContainer").modal("hide");
	});
	$(document).on("click", ".modal-refresh", function (e) {
		e.preventDefault();
		location.reload();
	});
	$(document).on("submit", ".modalForm", function (e) {
		e.preventDefault();
		$("#progress").show();
		$.ajax({
			url: this.action,
			type: this.method,
			data: new FormData(this),
			cache: false,
			contentType: false,
			processData: false,
            success: function (result) {
				if (result.code === 1) {
					$("#modalContainer").modal("hide");
					$("#progress").hide();
					if (result.ReturnUrl)
						location.href = result.ReturnUrl;
					else
						location.reload();
				} else {
					$("#progress").hide();
					$("#modalContainer").modal({ keyboard: true }, "show");
					$("#modalContent").html(result);
				}
			}
		});
	});
});
