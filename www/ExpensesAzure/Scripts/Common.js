
function getElOffsetLeft(element) {
	var x = 0, y = 0;
	var el = element;

	while (el.offsetLeft !== null) {
		x += el.offsetLeft;
		if (el.offsetParent === null)
			break;
		el = el.offsetParent;
	}

	el = element;
	while (el.offsetTop !== null) {
		y += el.offsetTop;
		if (el.offsetParent === null)
			break;
		el = el.offsetParent;
	}
	return [x, y];
}

function adjustElementHeight(el) {
	if (el !== null && typeof el !== "undefined") {
		var height = $(window).height();
		var offsets = getElOffsetLeft(el);

		//  Must use fixed bottom offset to provide space for the button.
		height = (height - offsets[1]) - 150;

		el.style.height = "" + height + "px";
	}
}

function adjustElementWidth(el) {
	if (el !== null && typeof el !== "undefined") {
		var main = document.getElementById("main");
		if (main !== null && typeof main !== "undefined") {
			var width = main.clientWidth;
			var mainOffsets = getElOffsetLeft(main);
			var offsets = getElOffsetLeft(el);

			width = (width - (offsets[0] - mainOffsets[0])) - 50;
			el.style.width = "" + width + "px";
		}
	}
}

function adjustElementSizes(el) {
	adjustElementHeight(el);
	adjustElementWidth(el);
}

function adjustHeaderWidth() {
	var owner = document.getElementById('ellipsisOwner');
	var inner = document.getElementById('ellipsisInner');

	if (isDefined(owner) && isDefined(inner)) {
		var width = owner.clientWidth;

		//	https://www.evernote.com/shard/s132/nl/14501366/3c9d9af6-e039-4066-a84f-40727a269ef9
		//  https://www.evernote.com/shard/s132/nl/14501366/aed21414-9496-417a-9336-a4f1b35585bf
		//	CSS is not ready at the time of script execution. 
		//	Must be consistent with CSS constant.
		var marginLeft = 15; //inner.style.marginLeft.substring(0, inner.style.marginLeft.search('px'));
		var marginRight = 15; //inner.style.marginRight.substring(0, inner.style.marginRight.search('px'));
		if (width > marginLeft + marginRight) width -= marginLeft + marginRight;
		else width = 0;
		inner.style.width = width.toString() + 'px';
	}
}

function deleteMultilpleAttr(el) {
	if (el !== null && typeof el !== "undefined") {
		el.multiple = false;
	}
}

function setInputValue(el, value) {
	if (el !== null && typeof el !== "undefined") {
		el.value = value;
	}
}

/*Fills the time by client time.*/
function fillDateElements() {
	var date = new Date();
	var hour = date.getHours();
	var min = date.getMinutes();
	var sec = date.getSeconds();
	var year = date.getFullYear();
	var month = date.getMonth() + 1;
	var day = date.getDate();

	var dayEl = document.getElementById("Day");
	var monthEl = document.getElementById("Month");
	var yearEl = document.getElementById("Year");
	var hourEl = document.getElementById("Hour");
	var minEl = document.getElementById("Min");
	var secEl = document.getElementById("Sec");

	//	https://www.evernote.com/shard/s132/nl/14501366/67b5959f-63bc-4cd5-af1a-a481a2859c50
	var startYearEl = document.getElementById("StartYear");
	var endYearEl = document.getElementById("EndYear");
	var startMonthEl = document.getElementById("StartMonth");
	var endMonthEl = document.getElementById("EndMonth");

	//	https://www.evernote.com/shard/s132/nl/14501366/cadee374-b60a-451f-bed5-d9237644dac3
	if (isDefined(dayEl) && (!isDefined(dayEl.value) || dayEl.value == -1)) setInputValue(dayEl, day);
	if (isDefined(monthEl) && (!isDefined(monthEl.value) || monthEl.value == -1)) setInputValue(monthEl, month);
	if (isDefined(yearEl) && (!isDefined(yearEl.value) || yearEl.value == -1)) setInputValue(yearEl, year);
	if (isDefined(hourEl) && (!isDefined(hourEl.value) || hourEl.value == -1)) setInputValue(hourEl, hour);
	if (isDefined(minEl) && (!isDefined(minEl.value) || minEl.value == -1)) setInputValue(minEl, min);
	if (isDefined(secEl) && (!isDefined(secEl.value) || secEl.value == -1)) setInputValue(secEl, sec);

	//	https://www.evernote.com/shard/s132/nl/14501366/67b5959f-63bc-4cd5-af1a-a481a2859c50
	if (isDefined(startYearEl) && (!isDefined(startYearEl.value) || startYearEl.value == -1)) setInputValue(startYearEl, year);
	if (isDefined(endYearEl) && (!isDefined(endYearEl.value) || endYearEl.value == -1)) setInputValue(endYearEl, year);
	if (isDefined(startMonthEl) && (!isDefined(startMonthEl.value) || startMonthEl.value == -1)) setInputValue(startMonthEl, month);
	if (isDefined(endMonthEl) && (!isDefined(endMonthEl.value) || endMonthEl.value == -1)) setInputValue(endMonthEl, month);
}

//	https://www.evernote.com/shard/s132/nl/14501366/e9be060a-5343-47e7-9441-65cbb5c80f60
function setResetFlag() {
	var reset = document.getElementById("Reset");

	if (isDefined(reset)) setInputValue(reset, 1);
}

//	https://www.evernote.com/shard/s132/nl/14501366/47e64199-5c58-43a1-9d0d-9d3081811def
function getYearMonthAsParams() {
	var monthEl = document.getElementById("Month");
	var yearEl = document.getElementById("Year");

	if (!isDefined(monthEl) || !isDefined(monthEl.value) || !isDefined(yearEl) || !isDefined(yearEl.value))
		return '';

	return '?year=' + yearEl.value + '&' + 'month=' + monthEl.value;
}

function initDatePicker() {
	//	https://www.evernote.com/shard/s132/nl/14501366/990cdfaa-11f8-4d6c-b83a-c139f78bdc53
	var dayEl = document.getElementById("Day");
	var monthEl = document.getElementById("Month");
	var yearEl = document.getElementById("Year");

	var date = new Date;
	if (isDefined(dayEl) && isDefined(dayEl.value) &&
		isDefined(monthEl) && isDefined(monthEl.value) &&
		isDefined(yearEl) && isDefined(yearEl.value))
		//	Must be consistent with date format below.
		date = '' + yearEl.value + '-' + monthEl.value + '-' + dayEl.value;

	$('.single').pickmeup({
		flat: true,
		date: date,
		format: 'Y-m-d',
		change: function (formattedDate) {
			var dateparts = formattedDate.split("-");
			var newDay = dateparts[2];
			var newMonth = dateparts[1];
			var newYear = dateparts[0];

			setInputValue(document.getElementById("Day"), newDay);
			setInputValue(document.getElementById("Month"), newMonth);
			setInputValue(document.getElementById("Year"), newYear);
			setInputValue(document.getElementById("Hour"), 0);
			setInputValue(document.getElementById("Min"), 0);
			setInputValue(document.getElementById("Sec"), 0);

			var el = document.getElementById("warning");
			if (el) {
				el.style.visibility = "visible";
				//	Text must be universal.
				//	https://www.evernote.com/shard/s132/nl/14501366/a7c9e99f-13f2-4ae1-b216-00dc674b2d09
				var months = ['jan', 'feb', 'mar', 'apr', 'may', 'jun', 'jul', 'aug', 'sep', 'oct', 'nov', 'dec'];
				var date = '' + newDay + ' ' + months[newMonth - 1]  + ' ' + newYear;
				el.innerHTML = date;
				updateParentHeight();
			}
		}
	});
	$('.single').pickmeup_twitter_bootstrap();

	//	https://www.evernote.com/shard/s132/nl/14501366/a7c9e99f-13f2-4ae1-b216-00dc674b2d09
	var months = ['jan', 'feb', 'mar', 'apr', 'may', 'jun', 'jul', 'aug', 'sep', 'oct', 'nov', 'dec'];

	$('.month_calendar').pickmeup({
		flat: true,
		date: date,
		view: 'months',
		select_day: false,
		format: 'Y-m',
		change: function (formattedDate) {
			var dateparts = formattedDate.split("-");
			var newMonth = dateparts[1];
			var newYear = dateparts[0];

			setInputValue(document.getElementById("Month"), newMonth);
			setInputValue(document.getElementById("Year"), newYear);

			var el = document.getElementById("warning");
			if (el) {
				el.style.visibility = "visible";
				var date = months[newMonth - 1]  + ' ' + newYear;
				el.innerHTML = date;
				updateParentHeight();
			}
		}
	});
	$('.month_calendar').pickmeup_twitter_bootstrap();

	//	https://www.evernote.com/shard/s132/nl/14501366/67b5959f-63bc-4cd5-af1a-a481a2859c50
	$('.start_month_calendar').pickmeup({
		flat: true,
		date: date,
		view: 'months',
		select_day: false,
		format: 'Y-m',
		change: function (formattedDate) {
			var dateparts = formattedDate.split("-");
			var newMonth = dateparts[1];
			var newYear = dateparts[0];

			setInputValue(document.getElementById("StartMonth"), newMonth);
			setInputValue(document.getElementById("StartYear"), newYear);

			var el = document.getElementById("start_month");
			if (el) {
				el.style.visibility = "visible";
				var date = months[newMonth - 1] + ' ' + newYear;
				el.innerHTML = date;
				updateParentHeight();
			}
		}
	});
	$('.start_month_calendar').pickmeup_twitter_bootstrap();

	$('.end_month_calendar').pickmeup({
		flat: true,
		date: date,
		view: 'months',
		select_day: false,
		format: 'Y-m',
		change: function (formattedDate) {
			var dateparts = formattedDate.split("-");
			var newMonth = dateparts[1];
			var newYear = dateparts[0];

			setInputValue(document.getElementById("EndMonth"), newMonth);
			setInputValue(document.getElementById("EndYear"), newYear);

			var el = document.getElementById("end_month");
			if (el) {
				el.style.visibility = "visible";
				var date = months[newMonth - 1] + ' ' + newYear;
				el.innerHTML = date;
				updateParentHeight();
			}
		}
	});
	$('.end_month_calendar').pickmeup_twitter_bootstrap();
};

/*
function fillDateElementsFromDatePicker() {
	//	Sets the date attributes by current time.
	var date = new Date();
	var hour = date.getHours();
	var min = date.getMinutes();
	var sec = date.getSeconds();
	var year = date.getFullYear();
	var month = date.getMonth() + 1;
	var day = date.getDate();
	
	setInputValue(document.getElementById("Day"), day);
	setInputValue(document.getElementById("Month"), month);
	setInputValue(document.getElementById("Year"), year);
	setInputValue(document.getElementById("Hour"), hour);
	setInputValue(document.getElementById("Min"), min);
	setInputValue(document.getElementById("Sec"), sec);

	var dateparts = $('.single').pickmeup('get_date', true).split("-");
	var newDay = dateparts[2];
	var newMonth = dateparts[1];
	var newYear = dateparts[0];

	//	If the user changes data on the calendar
	//	then uses this data to add the expenses. 
	//	In this case the time will be filled by zeros.
	if (day != newDay || month != newMonth || year != newYear) {
		setInputValue(document.getElementById("Day"), newDay);
		setInputValue(document.getElementById("Month"), newMonth);
		setInputValue(document.getElementById("Year"), newYear);
		setInputValue(document.getElementById("Hour"), 0);
		setInputValue(document.getElementById("Min"), 0);
		setInputValue(document.getElementById("Sec"), 0);
	}
}
*/

//	https://action.mindjet.com/task/14919145
function updateDiagramURL(pie, url, diagramId, diagramContainerId) {
	// ReSharper disable UseOfImplicitGlobalInFunctionScope
    var image = document.getElementById(diagramId);
	if (image && isDefined(url)) {
        var el = document.getElementById(diagramContainerId);
		if (!isDefined(el))
			el = $(".panel-body")[0];
		//	Square shape is better in most of the cases.
		//	https://www.evernote.com/shard/s132/nl/14501366/e0eb1c4e-4561-4da4-ae7c-5c26648ec6fc
		var imageHeight = /*194*/ $(el).width();
		var imageWidth = /*210*/  $(el).width();

		if (imageHeight > 640) imageHeight = 640;
		if (imageWidth > 640) imageWidth = 640;

		var src = url;
		if (url.indexOf("?") == -1)
			src += "?";
		else
			src += "&";
		src += "width=" + imageWidth + "&height=" + imageHeight + "&pie=" + (isDefined(pie) && pie ? "true" : "false");

		image.src = src;
	}
	// ReSharper restore UseOfImplicitGlobalInFunctionScope
}

function updateDiagram(pie) {
	if (imageURL !== undefined)
		updateDiagramURL(pie, imageURL, "diagram", "diagram_container");
}

function updateDiagramSize() {
	//	If the logic supports the diagram is rendered as pie-chart by default.
	//	https://www.evernote.com/shard/s132/nl/14501366/41e0b392-d4cb-4843-bf6d-2dea63b9c42f
	updateDiagram(true);
}

function alignElements() {
	document.getElementById('main').style.height = (screen.height - document.getElementById('footer').clientHeight).toString() + 'px';
	document.getElementById('main').style.width = screen.width.toString() + 'px';
	document.getElementById('framecontainer').style.height = (document.getElementById('main').clientHeight - document.getElementById('caption').clientHeight).toString() + 'px';
};

/*Fills the time by client time.*/
function fillDateParams(id) {
	var date = new Date();
	var hour = date.getHours();
	var min = date.getMinutes();
	var sec = date.getSeconds();
	var year = date.getFullYear();
	var month = date.getMonth() + 1;
	var day = date.getDate();

	var anchor = document.getElementById(id);
	if (anchor) {
		anchor.href += '&hour=' + hour + '&min=' + min + '&sec=' + sec + '&year=' + year + '&month=' + month + '&day=' + day;
	}
}

function isDefined(obj) {
	if (obj !== undefined && obj !== null) return true;
	else return false;
}

//	https://action.mindjet.com/task/14501087
var messageDestination = null;

function updateParentHeight() {
	if (isDefined(messageDestination)) {
		//  https://action.mindjet.com/task/14501085
		//  https://action.mindjet.com/task/14505157
		//  https://action.mindjet.com/task/14708792
		window.parent.postMessage(document.body.scrollHeight, messageDestination);
	}
}

//  Updates diagram's URL and resizes it.
function onDiagramResize() {
	updateDiagramSize();
	updateParentHeight();
}

function showJumbotron() {
	$('.page-content').each(function () { $(this).css('visibility', 'hidden') });
	$('.jumbotron').each(function () { $(this).css('visibility', 'visible') });
}

function deletePassword() {
	sessionStorage.removeItem("password");
	sessionStorage.removeItem("passwordConfirmed");
}

//	https://www.evernote.com/shard/s132/nl/14501366/5ea53405-2fc4-4166-a9e3-e918f3583785
function setPassword(url) {
	deletePassword();

	var p = document.getElementById("Password"); 
	if (isDefined(p) && isDefined(p.value) && p.value !== '') {
		sessionStorage["password"] = p.value;
		sessionStorage["passwordConfirmed"] = false;
		window.location.href = url;
	}
	else
		showJumbotron();
}

//	https://www.evernote.com/shard/s132/nl/14501366/5ea53405-2fc4-4166-a9e3-e918f3583785
function dropPassword(url) {
	deletePassword();
	window.location.href = url;
}

//	https://www.evernote.com/shard/s132/nl/14501366/5ea53405-2fc4-4166-a9e3-e918f3583785
function checkAndSetPassword(url) {
	deletePassword();

	//	Checks that two passwords are the same and shows message in the other case.
	var p = document.getElementById("Password");
	var p2 = document.getElementById("Password2");

	if (isDefined(p) && isDefined(p.value) && p.value !== '' &&
		isDefined(p2) && isDefined(p2.value) &&
		p2.value == p.value) {
		sessionStorage["password"] = p.value;
		sessionStorage["passwordConfirmed"] = true;

		window.location.href = url;
	}
	else
		showJumbotron();
}

//	https://www.evernote.com/shard/s132/nl/14501366/5ea53405-2fc4-4166-a9e3-e918f3583785
function encryptData(srcElId, encryptedElId) {
	var srcEl = document.getElementById(srcElId);
	var encryptedEl = document.getElementById(encryptedElId);
	var password = sessionStorage["password"];
	var passwordConfirmed = sessionStorage["passwordConfirmed"];

	if (isDefined(srcEl) && isDefined(srcEl.value) && (srcEl.value.trim() !== '') &&
		isDefined(encryptedEl) && isDefined(password) &&
		isDefined(passwordConfirmed) && passwordConfirmed === 'true') {
		try {
			var encryptedText = sjcl.encrypt(password, srcEl.value.trim());
			var formText = 'password required';
			srcEl.value = formText;
			//	Converts the object into base64-string.
			encryptedEl.value = window.btoa(encryptedText);
		}
		catch (e) {
		}
	}
	else
		showJumbotron();
}

//	https://www.evernote.com/shard/s132/nl/14501366/a5b2116f-ec6d-4803-81a2-6084a9c0a07d
function isDefinedPassword() {
	var password = sessionStorage["password"];
	return isDefined(password);
}

//	https://www.evernote.com/shard/s132/nl/14501366/5ea53405-2fc4-4166-a9e3-e918f3583785
function decryptData(destElId, base64EncrtypedData) {
	var destEl = document.getElementById(destElId);
	var password = sessionStorage["password"];

	if (isDefined(destEl) && isDefined(base64EncrtypedData) && isDefined(password)) {
		var json = window.atob(base64EncrtypedData);
		try {
			var decryptedText = sjcl.decrypt(password, json).trim();
			destEl.value = decryptedText;
			//	Don't touch this!
			//	https://www.evernote.com/shard/s132/nl/14501366/d03bc138-ab63-470b-8b99-df02ec42f205
			$(destEl).text(decryptedText);
		}
		catch (e)
		{
		}
	}
}

//	https://www.evernote.com/shard/s132/nl/14501366/d03bc138-ab63-470b-8b99-df02ec42f205
function decryptAllData() {
	//	https://www.evernote.com/shard/s132/nl/14501366/341528e8-b739-4539-a620-7d79de941df2
	if (typeof encryptedPairs !== 'undefined' && isDefined(encryptedPairs)) {
		var pairs = encryptedPairs['encryptedPairs'];
		for (var i in pairs) {
			var pair = pairs[i];
			decryptData(pair.Id, pair.EncryptedName);
		}
	}
}

//	https://www.evernote.com/shard/s132/nl/14501366/5ea53405-2fc4-4166-a9e3-e918f3583785
function encryptList(selector, updateMethod) {
	var password = sessionStorage["password"];
	var passwordConfirmed = sessionStorage["passwordConfirmed"];

	if (isDefined(password) && isDefined(updateMethod) &&
		isDefined(passwordConfirmed) && passwordConfirmed === 'true') {

		//	https://www.evernote.com/shard/s132/nl/14501366/b0cd20cc-1706-40fc-96ad-054932a2bc7f
		$('#progressDialog').modal('show');

		var count = 0;
		$(selector).each(function () { count++; });

		$("#progressBar").attr('aria-valuenow', "0");
		$("#progressBar").css('width', '0%');
		$("#progressText").text = '0%';

		var processed = 0;

		$(selector).each(function () {
			var decodedText = $(this).text().trim();
			if (decodedText !== '') {
				//	https://www.evernote.com/shard/s132/nl/14501366/eac2dc0a-f87d-4a6d-9fd6-3329b2f11a71
				var encryptedList = [];
				var item = {};
				item.id = $(this).attr('id');
				item.encryptedText = window.btoa(sjcl.encrypt(password, decodedText));
				item.openText = 'password required';
				encryptedList.push(item);
				//	https://www.evernote.com/shard/s132/nl/14501366/eac2dc0a-f87d-4a6d-9fd6-3329b2f11a71
				var encryptedObj = {};
				encryptedObj.list = encryptedList;
				var s = window.btoa(JSON.stringify(encryptedObj));
				updateMethod(s);

				processed++;
				var percent = Math.floor(processed * 100.0 / count);
				$("#progressBar").attr('aria-valuenow', percent.toString());
				$("#progressBar").css('width', percent + '%');
				$("#progressText").text = percent + '%';
			}
		});

		//	https://www.evernote.com/shard/s132/nl/14501366/b0cd20cc-1706-40fc-96ad-054932a2bc7f
		$('#progressDialog').modal('hide');
	}
	else
		showJumbotron();
}

function updateContentHeight() {
	var jumbotron = $(".jumbotron")[0];
	var minHeight = $(jumbotron).height();
	$('.page-content').each(function () {
		$(this).css('min-height', minHeight + 'px');
	});
}

//	https://www.evernote.com/shard/s132/nl/14501366/5ea53405-2fc4-4166-a9e3-e918f3583785
function setLockIndicator() {
	var password = sessionStorage["password"];
	var passwordConfirmed = sessionStorage["passwordConfirmed"];

	if (isDefined(password)) {

		if (isDefined(passwordConfirmed) && passwordConfirmed === 'true') {
			$('#lockIndicator').each(function () {
				$(this).removeClass("btn-default btn-warning btn-info").addClass("btn-success");
			});
		}
		else {
			$('#lockIndicator').each(function () {
				$(this).removeClass("btn-default btn-warning btn-success").addClass("btn-info");
			});
		}
	}
	else {
		$('#lockIndicator').each(function () {
			$(this).removeClass("btn-default btn-info btn-success").addClass("btn-warning");
		});
	}
}

//	https://www.evernote.com/shard/s132/nl/14501366/5b6f473a-b5ec-4a62-adf2-17362aea5d81
function onCurrencyCodeClick(code) {
	if (isDefined(sessionStorage["selectCurrencyReturnUrl"])) {
		//	Name must coincide with the element id. See below.
		sessionStorage["Currency"] = code;
		sessionStorage["restoreDataFromSession"] = true;
		window.location.href = sessionStorage["selectCurrencyReturnUrl"];
	}
}

//	https://www.evernote.com/shard/s132/nl/14501366/5b6f473a-b5ec-4a62-adf2-17362aea5d81
function fillCurrencyCodes() {
	var codesTable = document.getElementById("currencyCodes");

	if (codesTable) {
		var codes = [
			'USD', 'EUR', 'GBP', 'CNY', 'JPY', 'INR', 'RUB', 'RUR', 'AED', 'AFN', 'ALL', 'AMD', 'ANG', 'AOA', 'ARS',
			'AUD', 'AWG', 'AZN', 'BAM', 'BBD', 'BDT', 'BGN', 'BHD', 'BIF', 'BMD', 'BND', 'BOB', 'BRL', 'BSD', 'BTN',
			'BWP', 'BYR', 'BZD', 'CAD', 'CDF', 'CHF', 'CLP', 'COP', 'CRC', 'CUP', 'CVE', 'CZK', 'DJF', 'DKK', 'DOP',
			'DZD', 'EGP', 'ERN', 'ETB', 'FJD', 'FKP', 'GEL', 'GHS', 'GIP', 'GMD', 'GNF', 'GTQ', 'GYD', 'HKD', 'HNL',
			'HRK', 'HTG', 'HUF', 'IDR', 'ILS', 'IQD', 'IRR', 'ISK', 'JMD', 'JOD', 'KES', 'KGS', 'KHR', 'KMF', 'KPW',
			'KWD', 'KYD', 'KZT', 'LAK', 'LBP', 'LKR', 'LRD', 'LSL', 'LTL', 'LVL', 'LYD', 'MAD', 'MDL', 'MGA', 'MKD',
			'MMK', 'MNT', 'MOP', 'MRO', 'MUR', 'MVR', 'MWK', 'MXN', 'MYR', 'MZN', 'NAD', 'NGN', 'NIO', 'NOK', 'NPR',
			'NZD', 'OMR', 'PAB', 'PEN', 'PGK', 'PHP', 'PKR', 'PLN', 'PYG', 'QAR', 'RON', 'RSD', 'RWF', 'SAR', 'SBD',
			'SCR', 'SDG', 'SEK', 'SGD', 'SHP', 'SLL', 'SOS', 'SRD', 'STD', 'SYP', 'SZL', 'THB', 'TJS', 'TMT', 'TND',
			'TOP', 'TRY', 'TTD', 'TWD', 'TZS', 'UAH', 'UGX', 'UYU', 'UZS', 'VEF', 'VND', 'VUV', 'WST', 'XAF', 'XCD',
			'XOF', 'XPF', 'YER', 'ZAR', 'ZMK', 'ZWD'];

		var rowButtonNum = 4;

		var rowNum = codes.length / rowButtonNum + (codes.length % rowButtonNum != 0 ? 1 : 0);
		for (var i = 0; i < rowNum; i++)
		{
			var row = '';
			for (var j = 0; j < rowButtonNum; j++)
			{
				var index = i * rowButtonNum + j;
				if (index < codes.length)
				{
					row += '<td><button type="button" class="btn btn-default btnCode" onclick="onCurrencyCodeClick(\'' + codes[index] + '\');">' + codes[index] + '</button></td>';
				}
			}
			codesTable.insertRow(-1).innerHTML = row;
		}
	}
}

//	https://www.evernote.com/shard/s132/nl/14501366/5b6f473a-b5ec-4a62-adf2-17362aea5d81
function fillCurrencyChars() {
	var codesTable = document.getElementById("currencyChars");

	if (codesTable) {
		var codes = ['$', '€', '£', '¥', '₽', '₩', '₴', '₲', 'ƒ', '₫', '₭', '₡', '₺', '₦', '₱', '﷼', '៛', '₹', '₨', '₵', '৳', '₸', '₮', '₪', '؋', '฿'];

		var rowButtonNum = 5;

		var rowNum = codes.length / rowButtonNum + (codes.length % rowButtonNum != 0 ? 1 : 0);
		for (var i = 0; i < rowNum; i++) {
			var row = '';
			for (var j = 0; j < rowButtonNum; j++) {
				var index = i * rowButtonNum + j;
				if (index < codes.length) {
					row += '<td><button type="button" class="btn btn-default btnCurrency" onclick="onCurrencyCodeClick(\'' + codes[index] + '\');">' + codes[index] + '</button></td>';
				}
			}
			codesTable.insertRow(-1).innerHTML = row;
		}
	}
}

var elements = ["Day", "Month", "Year", "Hour", "Min", "Sec", "StartYear", "StartMonth", "EndYear", "EndMonth", "Name", "Cost", "Currency", "EncryptedName"];

//	https://www.evernote.com/shard/s132/nl/14501366/5b6f473a-b5ec-4a62-adf2-17362aea5d81
function saveDataIntoSession() {
	for (var i = 0; i < elements.length; i++) {
		var elementName = elements[i];
		var el = document.getElementById(elementName);

		if (isDefined(el) && isDefined(el.value)) {
			sessionStorage[elementName] = el.value;
		}
	}
}

//	https://www.evernote.com/shard/s132/nl/14501366/5b6f473a-b5ec-4a62-adf2-17362aea5d81
function restoreDataFromSession() {
	if (isDefined(sessionStorage["restoreDataFromSession"])) {
		for (var i = 0; i < elements.length; i++) {
			var elementName = elements[i];

			if (isDefined(sessionStorage[elementName])) {

				var el = document.getElementById(elementName);
				if (isDefined(el)) {
					el.value = sessionStorage[elementName];
				}
				sessionStorage.removeItem(elementName);
			}
		}

		sessionStorage.removeItem("restoreDataFromSession");
	}
}

//	https://www.evernote.com/shard/s132/nl/14501366/5b6f473a-b5ec-4a62-adf2-17362aea5d81
function onSelectCurrencyClick() {
	if (isDefined(selectCurrencyUrl)) {
		sessionStorage["selectCurrencyReturnUrl"] = window.location.href;
		saveDataIntoSession();
		window.location.href = selectCurrencyUrl;
	}
}

function initTooltips() {
	$('.tooltipButton').tooltip();
}

//	https://www.evernote.com/shard/s132/nl/14501366/f403d09e-519b-4121-b3d8-b38d476fe462
function joinListItems() {
	if (isDefinedPassword()) {
		var items = [];
		$(".encrypted, .unencrypted").not(".disjoined").each(function () { items.push($(this)); });

		for (var i in items)
		{
			var newId = items[i].attr('id');
			var newEl = document.getElementById(newId);
			if (!isDefined(newEl)) continue;

			for (var j in items) {
				var existingId = items[j].attr('id');
				var existingEl = document.getElementById(existingId);
				if (!isDefined(existingEl)) continue;

				if (existingEl.innerHTML.trim() === newEl.innerHTML.trim() && existingId !== newId) {
					var removedId = (newId < existingId) ? newId : existingId;
					var savedId = (newId < existingId) ? existingId : newId;
					//	Search for total and sums if exists.
					var removedTotal = document.getElementById("ttl" + removedId);
					var savedTotal = document.getElementById("ttl" + savedId);

					if (isDefined(removedTotal) && isDefined(savedTotal)) {
						var total = 0.0;
						var val1 = Number(removedTotal.innerHTML);
						var val2 = Number(savedTotal.innerHTML);
						total += val1;
						total += val2;
						savedTotal.innerHTML = total.toFixed(2);
					}

					$("#tr" + removedId).remove();
				}
			};
		};
	}
}

//	https://www.evernote.com/shard/s132/nl/14501366/f403d09e-519b-4121-b3d8-b38d476fe462
function decryptListData(newElId, base64EncrtypedData)
{
	decryptData(newElId, base64EncrtypedData);
}

//	https://www.evernote.com/shard/s132/nl/14501366/748e493d-9338-41b0-adfb-a9b40cbdb4dd
function sortDecryptedList() {
	$('.sorted').each(function () {
		var elements = [];
		var sortedTable = $(this);

		$('.encrypted, .unencrypted').each(function () {
			var id = $(this).attr('id');
			var text = $(this).text();
			//	Clones the parent row.
			var row = $("#tr" + id).clone();

			var obj = { id: id, text: text, row: row };
			elements.push(obj);
		});
		elements.sort(function (a, b) {
			return a.text.trim() == b.text.trim()
					? 0
					: (a.text.trim() > b.text.trim() ? 1 : -1);
		});
		//	Remove unsorted rows with encrypted data.
		$('.trlist').remove();

		$.each(elements, function (index, value) {
			sortedTable.children('tbody').append(value.row);
		});
		$(this).css('visibility', 'visible');
	});
}

//	https://www.evernote.com/shard/s132/nl/14501366/38cbc348-7f74-4536-ba4a-9653e751b59e
function showDecryptedList() {
	$('.encrypted').each(function () {
		$(this).css('visibility', 'visible');
	});
}

(function () {
	//	https://action.mindjet.com/task/14501087
	var messageEventHandler = function (event) {
		if (isDefined(event.data)) {
			messageDestination = event.data;
			updateParentHeight();
		}
	};
	window.addEventListener('message', messageEventHandler, false);

	document.addEventListener("DOMContentLoaded", function () {
		//alignElements();
		fillDateElements();
		initDatePicker();
		updateDiagramSize();
		window.onresize = onDiagramResize;
		adjustHeaderWidth();
		updateContentHeight();
		setLockIndicator();
		fillCurrencyCodes();
		fillCurrencyChars();
		restoreDataFromSession();
		initTooltips();
	});
	$(window).load(function () {
		decryptAllData();
		joinListItems();
		sortDecryptedList();
		showDecryptedList();
	});
})();
