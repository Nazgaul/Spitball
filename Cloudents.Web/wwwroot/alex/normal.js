/******************
   Known Issues
******************/

/* The CSS spec mandates that the translateX/Y/Z transforms are %-relative to the element itself -- not its parent.
Velocity, however, doesn't make this distinction. Thus, converting to or from the % unit with these subproperties
will produce an inaccurate conversion value. The same issue exists with the cx/cy attributes of SVG circles and ellipses. */
(function($, window, document, undefined) {

	"use strict";
	var Alex = {
		meta: {
			$doc: $(document),
			$win: $(window),
			wHeight: 0,
			wWidth: 0,
			easingInValue: "easeOutCubic",
			easingOutValue: "easeInOut",
			isMobile: false,
			currentSection: "Section1",
			fullPageInterval: null,
			fullPageWaitTime: 1000
		},
		ui: {
			$html: $("html"),
			$body: $("body"),
			$initialImages: $(".init-image"),
			$headerWrapper: $(".header-wrapper"),
			$pageOverlayWrapper: $(".page-overlay-wrapper"),
			$overlaySections: $(".overlay-sections"),
			$overlaySection: $(".overlay-section"),
			$scrollSectionContent: $(".scroll-section-content"),
			$scrollSections: $(".scroll-section"),
			$scrollContainer: $(".scroll-container"),
			$textSlides: $(".text-slide"),
			$vid: $(".prod-video"),
			$imageSlides: $(".image-slide"),
			$lazyImgs: $(".js-load-async"),
			$faderLinks: $(".fader-link"),
			$gaTrackItems: $(".js-ga-track"),
			$fadeGradient: $(".fade-gradient")
		},
		init: function() {
			Alex.meta.$win.resize(function() {
				Alex.setWindowDimensions();
			}).trigger("resize");

			Alex.initMobileNav();

			if (/Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent)) {
				Alex.ui.$html.addClass("is-mobile");
				Alex.meta.isMobile = true;
				var pageElem = ".fixed-content";
				if (Alex.meta.wWidth > 768) {
					pageElem = ".scroll-container";
				}
				$(pageElem).fullpage({
					sectionSelector: ".scroll-section",
					fitToSection: false,
					verticalCentered: false,
					scrollingSpeed: 1000,
					loopBottom: false,
					loopTop: false,
					easing: "easeInOutCubic"
				});

			}
			if ($(".home-page").length) {
				if (Alex.meta.wWidth >= 768 && !Alex.meta.isMobile) {
					Alex.ui.$scrollSectionContent.height(Alex.meta.wHeight);
				} else if (Alex.meta.wWidth >= 768 && Alex.meta.isMobile) {
					Alex.meta.$win.resize(function() {
						Alex.ui.$body.height(Alex.meta.wHeight);
						Alex.ui.$scrollContainer.height(Alex.meta.wHeight);
					}).trigger("resize");
				}

				setTimeout(function() {
						Alex.ui.$headerWrapper.addClass("active");
						Alex.ui.$fadeGradient.addClass("active");
					},
					1000);


				Alex.ui.$initialImages.imagesLoaded()
					.done(function() {
						Alex.ui.$initialImages.velocity({
								opacity: 1.0,
								//translateZ: 0
							},
							{
								complete: function() {
									setTimeout(function() {
											Alex.initHomePage();
										},
										50);
								}
							});
					})
					.fail();
			}
		},
		setWindowDimensions: function() {
			Alex.meta.wHeight = Alex.meta.$win.outerHeight(true);
			Alex.meta.wWidth = Alex.meta.$win.outerWidth(true);
		},
		handleHash: function() {
			var destHash = "",
				$fadeIns = Alex.ui.$overlaySection.find(".fade-in"),
				$targetLink,
				overlayTarget;

			Alex.meta.$win.on("hashchange",
				function() {
					// Close overlay.
					if (window.location.hash === "") {
						Alex.ui.$headerWrapper.removeClass("overlay-is-active");
						Alex.ui.$faderLinks.removeClass("active");

						$fadeIns
							.velocity({
									opacity: 0.0,
									translateZ: 0
								},
								{
									duration: 300,
									complete: function() {
										Alex.ui.$pageOverlayWrapper
											.velocity({
													opacity: [0.0, 1.0],
													translateZ: 0
												},
												{
													duration: 300,
													display: "none",
													complete: function() {
														Alex.ui.$overlaySection.hide();

														if (!Alex.meta.isMobile) {
														$.fn.fullpage.setAllowScrolling(true);
														}

														$fadeIns.css("opacity", "0.0");
													}
												});

									}
								});

					} else {
						// Open or keep open overlay.
						destHash = window.location.hash.split("/")[1];
						$targetLink = $("." + destHash + "-header-link");
						overlayTarget = $targetLink.data("overlay-target");

						Alex.ui.$faderLinks.removeClass("active");
						$targetLink.addClass("active");

						if (Alex.ui.$headerWrapper.hasClass("overlay-is-active")) {
							$fadeIns
								.velocity({
										opacity: 0.0,
										translateZ: 0
									},
									{
										duration: 300,
										complete: function() {
											Alex.ui.$pageOverlayWrapper.scrollTop(0);
											Alex.ui.$overlaySection.hide();
											$("." + overlayTarget)
												.show()
												.find(".fade-in")
												.each(function(index) {
													var $this = $(this);

													$this.velocity({
															opacity: [1.0, 0],
															translateY: ["0", "30px"],
															translateZ: 0
														},
														{
															duration: 300,
															delay: (index + 1) * 250,
															easing: Alex.meta.easingInValue
														});
												});
										}
									});

							// If the overlay is not yet active.
						} else {
							Alex.ui.$headerWrapper.addClass("overlay-is-active");
							Alex.ui.$pageOverlayWrapper
								.velocity({
										opacity: [1.0, 0],
										translateZ: 0
									},
									{
										duration: 300,
										display: "block",
										complete: function() {
											Alex.ui.$pageOverlayWrapper.scrollTop(0);
											$.fn.fullpage.setAllowScrolling(false);
											$("." + overlayTarget)
												.show()
												.find(".fade-in")
												.each(function(index) {
													var $this = $(this);

													$this.velocity({
															opacity: [1.0, 0.0],
															translateY: ["0", "30px"],
															translateZ: 0
														},
														{
															duration: 300,
															delay: (index + 1) * 250,
															easing: Alex.meta.easingInValue
														});
												});
										}
									});
						}
					}
				}).trigger("hashchange");
		},
		initMobileNav: function() {
			$(".menu").on("click",
				function(e) {
					e.preventDefault();

					var $this = $(this),
						$mobileNav = $(".mobile-nav");

					if (!$this.hasClass("active")) {
						$this.addClass("active");
						$mobileNav.css("opacity", 1);
						$mobileNav.velocity({
								translateY: [0, "-100%"],
								translateZ: 0
							},
							{
								duration: 500,
								easing: Alex.meta.easingInValue
							});
					} else {
						$this.removeClass("active");
						$mobileNav.velocity({
								translateY: ["-100%", 0],
								translateZ: 0,
								opacity: 0
							},
							{
								duration: 500,
								easing: Alex.meta.easingOutValue
							});
					}
				});
		},
		initHomePage: function() {
			var textIndex = 0;

			if (Alex.meta.wWidth >= 768 && !Alex.meta.isMobile) {
				//Alex.loadLazyImages();
				Alex.initFaderNav();
				Alex.handleHash();
				Alex.ui.$scrollContainer.fullpage({
					sectionSelector: ".scroll-section",
					fitToSection: false,
					verticalCentered: false,
					scrollingSpeed: 1500,
					loopBottom: false,
					loopTop: false,
					easing: "easeInOutCubic",
					afterRender: function() {
						var $downArrow = $(".down-arrow");

						//Alex.meta.fullPageInterval = setInterval(function() {
						//		$downArrow.velocity("finish").velocity({
						//				translateY: "15px"
						//			},
						//			{
						//				duration: 800,
						//				easing: Alex.meta.easingInValue,
						//				complete: function() {
						//					$downArrow.velocity("finish").velocity({
						//							translateY: "0"
						//						},
						//						{
						//							duration: 500,
						//							easing: Alex.meta.easingOutValue
						//						});
						//				}
						//			});

						//	},
						//	Alex.meta.fullPageWaitTime);
					},
					onLeave: function(index, nextIndex, direction) {
						clearTimeout(Alex.meta.fullPageInterval);
						// var $currentSection;
						Alex.meta.currentSection = "Section" + nextIndex;
					}
				});


				$(".down-arrow").on("click",
					function(e) {
						e.preventDefault();

						Alex.meta.fullPageHardStop = true;
						$.fn.fullpage.moveSectionDown();
					});
				$("[data-section]").on("click",
					function (e) {
						e.preventDefault();
						$(".scroll-section").removeClass("active");
						var slideNum = $(this).data("section");
						$('body').removeClass("fp-viewing-0").addClass("fp-viewing-" + slideNum);
						$(".scroll-section:nth-of-type(" + slideNum + ")").addClass("active");
						Alex.ui.$scrollContainer.css("transform", "translate3d(0px," + -slideNum * 6000 + "px,0px)");
						//Alex.ui.$scrollContainer.css("transition", "transform 1s ease");
					});

			} else if (Alex.meta.wWidth >= 768 && Alex.meta.isMobile) {
				//Alex.meta.currentSection = "MobileView";
				Alex.initFaderNav();
				Alex.handleHash();

			}

		},
		loadMobileImages: function() {
			Alex.ui.$imageSlides.each(function() {
				var $this = $(this),
					imgSource = $this.data("src");

				$this.attr("src", imgSource);
			});
		},
		loadLazyImages: function() {
			Alex.ui.$lazyImgs.each(function(index) {
				var $this = $(this),
					imgSource = $this.data("image-src");

				$this
					.attr("src", imgSource);
				//$this.velocity({
				//		opacity: 1.0,
				//		translateZ: 0
				//	},
				//	{
				//		duration: 300,
				//		visibility: "visible",
				//		easing: Alex.easingInValue,
				//		delay: index * 250
				//	});
			});
		},
		initFaderNav: function() {
			Alex.ui.$faderLinks.on("click",
				function(e) {
					e.preventDefault();

					Alex.meta.fullPageHardStop = true;

					var $this = $(this),
						newHash = $this.data("overlay-target") ? $this.data("overlay-target").split("-")[0] : "";

					if ($this.hasClass("close-overlay")) {
						if (!Alex.ui.$headerWrapper.hasClass("overlay-is-active")) {
							window.location.href = "";
						}
					}

					window.location.hash = newHash ? "/" + newHash : "";
				});
		}
	};

	$(function() {
		Alex.init();
	});

})(jQuery, window, document);
