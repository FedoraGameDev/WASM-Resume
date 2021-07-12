lastKnownPosition = 0;
allowFurtherScroll = false;

document.addEventListener("scroll", () => {
	if (!allowFurtherScroll) {
		window.scrollTo(0, 100);

		if (lastKnownPosition === 0)
			setTimeout(() => allowFurtherScroll = true, 1000);
	}

	document.documentElement.dataset.scroll = Math.floor(window.scrollY);
	lastKnownPosition = window.scrollY;

	if (lastKnownPosition === 0)
		allowFurtherScroll = false;
});

function ScrollToPoint(targetPoint) {
	window.scrollTo(0, targetPoint);
}

function ScrollToFirstCard() {
	document.getElementById("first-card").scrollIntoView();
}