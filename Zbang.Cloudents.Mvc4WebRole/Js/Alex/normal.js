/******************
   Known Issues
******************/

/* The CSS spec mandates that the translateX/Y/Z transforms are %-relative to the element itself -- not its parent.
Velocity, however, doesn't make this distinction. Thus, converting to or from the % unit with these subproperties
will produce an inaccurate conversion value. The same issue exists with the cx/cy attributes of SVG circles and ellipses. */
(function ($,window, document, undefined) {

	'use strict';
	var Alex = {
		meta: {
			$doc: $(document),
			$win: $(window),
			wHeight: 0,
			wWidth: 0,
			easingInValue: 'easeOutCubic',
			easingOutValue: 'easeInOut',
			isMobile: false,
			currentSection: 'Section1',
			fullPageInterval: null,
			fullPageWaitTime: 2000
		},
		ui: {
			$html: $('html'),
			$body: $('body'),
			$initialImages: $('.init-image'),
			$headerWrapper: $('.header-wrapper'),
			$pageOverlayWrapper: $('.page-overlay-wrapper'),
			$overlaySections: $('.overlay-sections'),
			$overlaySection: $('.overlay-section'),
			$scrollSectionContent: $('.scroll-section-content'),
			$scrollSections: $('.scroll-section'),
			$scrollContainer: $('.scroll-container'),
			$textSlides: $('.text-slide'),
			$vid: $('.prod-video'),
			$imageSlides: $('.image-slide'),
			$lazyImgs: $('.js-load-async'),
			$faderLinks: $('.fader-link'),
			$gaTrackItems: $('.js-ga-track'),
			$fadeGradient: $('.fade-gradient')
		},
		init: function() {
			Alex.meta.$win.resize(function() {
				Alex.setWindowDimensions();
			}).trigger('resize');

			Alex.initGATracking();
			Alex.initMobileNav();

			if( /Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent) ) {
				Alex.ui.$html.addClass('is-mobile');
				Alex.meta.isMobile = true;
			}

			if ($('.home-page').length) {
				if (Alex.meta.wWidth >= 768 && !Alex.meta.isMobile) {
					Alex.ui.$scrollSectionContent.height(Alex.meta.wHeight);
				} else if (Alex.meta.wWidth >= 768 && Alex.meta.isMobile) {
					Alex.meta.$win.resize(function() {
						Alex.ui.$body.height(Alex.meta.wHeight);
						Alex.ui.$scrollContainer.height(Alex.meta.wHeight);
					}).trigger('resize');
				} 

				setTimeout(function() {
					Alex.ui.$headerWrapper.addClass('active');
					Alex.ui.$fadeGradient.addClass('active');          
				}, 1000);

        

				Alex.ui.$initialImages.imagesLoaded()
					.done( function() {            
						Alex.ui.$initialImages.velocity({
							opacity: 1.0,
							translateZ: 0
						}, {
							duration: 300,
							complete: function() {
								setTimeout(function() {
									Alex.initHomePage();
								}, 250);                
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
			var destHash = '',
				$fadeIns = Alex.ui.$overlaySection.find('.fade-in'),
				$targetLink,
				overlayTarget;

			Alex.meta.$win.on('hashchange', function() {
				// Close overlay.
				if (window.location.hash === '') {
					Alex.ui.$headerWrapper.removeClass('overlay-is-active');
					Alex.ui.$faderLinks.removeClass('active');

					$fadeIns
						.velocity({
							opacity: 0.0,
							translateZ: 0
						}, {
							duration: 300,
							complete: function() {
								Alex.ui.$pageOverlayWrapper
									.velocity({
										opacity: [0.0, 1.0],
										translateZ: 0       
									}, {
										duration: 300,
										display: 'none',
										complete: function() {
											Alex.ui.$overlaySection.hide();                

											if (!Alex.meta.isMobile) {
												$.fn.fullpage.setAllowScrolling(true);  
											}

											$fadeIns.css('opacity', '0.0');
										}
									}); 

							}
						});

				} else {
					// Open or keep open overlay.
					destHash = window.location.hash.split('/')[1];
					$targetLink = $('.' + destHash + '-header-link');
					overlayTarget = $targetLink.data('overlay-target');

					Alex.ui.$faderLinks.removeClass('active');
					$targetLink.addClass('active');

					if (Alex.ui.$headerWrapper.hasClass('overlay-is-active')) {
						$fadeIns
							.velocity({
								opacity: 0.0,
								translateZ: 0
							}, {
								duration: 300,
								complete: function() {
									Alex.ui.$pageOverlayWrapper.scrollTop(0);
									Alex.ui.$overlaySection.hide();
									$('.' + overlayTarget)
										.show()
										.find('.fade-in')
										.each(function(index) {
											var $this = $(this);

											$this.velocity({
												opacity: [1.0, 0],
												translateY: ['0', '30px'],
												translateZ: 0
											}, {
												duration: 300, 
												delay: (index + 1) * 250,
												easing: Alex.meta.easingInValue
											});
										});
								}
							});
            
						// If the overlay is not yet active.
					} else {
						Alex.ui.$headerWrapper.addClass('overlay-is-active');
						Alex.ui.$pageOverlayWrapper
							.velocity({
								opacity: [1.0, 0],
								translateZ: 0          
							}, {
								duration: 300,
								display: 'block',
								complete: function() {
									Alex.ui.$pageOverlayWrapper.scrollTop(0);
									if (!Alex.meta.isMobile) {
										$.fn.fullpage.setAllowScrolling(false);
									}

									$('.' + overlayTarget)
										.show()
										.find('.fade-in')
										.each(function(index) {
											var $this = $(this);

											$this.velocity({
												opacity: [1.0, 0.0],
												translateY: ['0', '30px'],
												translateZ: 0
											}, {
												duration: 300, 
												delay: (index + 1) * 250,
												easing: Alex.meta.easingInValue
											});
										});
								}
							});
					}
				}
			}).trigger('hashchange');    
		},
		initMobileNav: function() {
			$('.menu').on('click', function(e) {
				e.preventDefault();

				var $this = $(this),
					$mobileNav = $('.mobile-nav');

				if (!$this.hasClass('active')) {
					$this.addClass('active');
					$mobileNav.velocity({
						translateY: [0, '-100%'],
						translateZ: 0
					}, {
						duration: 500,
						easing: Alex.meta.easingInValue
					});
				} else {
					$this.removeClass('active');
					$mobileNav.velocity({
						translateY: ['-100%', 0],
						translateZ: 0
					}, {
						duration: 500,
						easing: Alex.meta.easingOutValue
					});
				}
			});
		},
		initHomePage: function() {
			var textIndex = 0,
				numSlides = Alex.ui.$textSlides.length,
				textInterval;

			//Alex.loadMobileImages();
			Alex.ui.$imageSlides
				.eq(0)
				.velocity('finish')
				.velocity({
					opacity: 1.0,
					translateZ: 0
				}, {
					duration: 300,
					visibility: 'visible'
				});
			// Init Fullpage JS.
			Alex.ui.$scrollContainer.fullpage({
				sectionSelector: '.scroll-section',
				fitToSection: false,
				verticalCentered: false,
				scrollingSpeed: 1500,
				loopBottom: false,
				loopTop: false,
				easing: 'easeInOutCubic',
				afterRender: function () {
					var $downArrow = $('.down-arrow');

					Alex.meta.fullPageInterval = setInterval(function () {
						$downArrow.velocity('finish').velocity({
							translateY: '15px'
						}, {
							duration: 800,
							easing: Alex.meta.easingInValue,
							complete: function () {
								$downArrow.velocity('finish').velocity({
									translateY: '0'
								}, {
									duration: 500,
									easing: Alex.meta.easingOutValue
								});
							}
						});

					}, Alex.meta.fullPageWaitTime);
				},
				onLeave: function (index, nextIndex, direction) {
					clearTimeout(Alex.meta.fullPageInterval);
					// var $currentSection;
					Alex.meta.currentSection = 'Section' + nextIndex;
					//ga('send', 'event', Alex.meta.currentSection, 'View', '');

					if (direction === 'down') {
						Alex.ui.$textSlides.eq(index - 1)
							.find('.js-animate-this')
							.velocity({
								opacity: 0.0,
								translateZ: 0,
								translateY: '-30px'
							}, {
								duration: 500,
								easing: Alex.meta.easingInValue,
								complete: function () {
									Alex.ui.$textSlides.eq(index - 1).removeClass('active');
								}
							});

						Alex.ui.$textSlides.eq(nextIndex - 1)
							.find('.js-animate-this')
							.each(function (index) {
								var $this = $(this);

								$this
									.velocity({
										opacity: 1.0,
										translateZ: 0,
										translateY: ['0', '30px']
									}, {
										delay: (index + 1) * 250,
										duration: 500,
										easing: Alex.meta.easingInValue,
										complete: function () {
											Alex.ui.$textSlides.eq(nextIndex - 1).addClass('active');
										}
									});
							});

						Alex.ui.$imageSlides
							.eq(nextIndex - 1)
							.velocity('finish')
							.velocity({
								translateY: ['0', '100%'],
								translateZ: 0
							}, {
								duration: 600,
								delay: 800,
								easing: Alex.meta.easingInValue,
								visibility: 'visible'
							});

						Alex.ui.$imageSlides
							.eq(index - 1)
							.velocity({
								translateZ: 0
							}, {
								delay: 1450,
								duration: 0,
								visibility: 'hidden'
							});

					} else {
						Alex.ui.$textSlides.eq(index - 1)
							.find('.js-animate-this')
							.velocity({
								opacity: 0.0,
								translateY: '30px',
								translateZ: 0
							}, {
								duration: 500,
								easing: Alex.meta.easingInValue,
								complete: function () {
									Alex.ui.$textSlides.eq(index - 1).removeClass('active');
								}
							});

						Alex.ui.$textSlides.eq(nextIndex - 1)
							.find('.js-animate-this')
							.each(function (index) {
								var $this = $(this);

								$this
									.velocity('finish')
									.velocity({
										opacity: 1.0,
										translateY: ['0', '-10%'],
										translateZ: 0
									}, {
										delay: (index + 1) * 250,
										easing: Alex.meta.easingInValue,
										duration: 500,
										complete: function () {
											Alex.ui.$textSlides.eq(nextIndex - 1).addClass('active');
										}
									});
							});

						Alex.ui.$imageSlides
							.eq(index - 1)
							.velocity('finish')
							.velocity({
								translateY: '100%',
								translateZ: 0
							}, {
								duration: 600,
								delay: 600,
								easing: Alex.meta.easingOutValue,
								visibility: 'hidden'
							});

						Alex.ui.$imageSlides
							.eq(nextIndex - 1)
							.velocity('finish')
							.velocity({
								translateZ: 0
							}, {
								duration: 0,
								delay: 0,
								visibility: 'visible'
							});
					}
				}
			});
			if (Alex.meta.wWidth >= 768 && !Alex.meta.isMobile) {
				Alex.loadLazyImages();
				Alex.initFaderNav();
				Alex.handleHash();
				// Animate first text slide in.
				Alex.ui.$textSlides.eq(0)
					.find('.js-animate-this')
					.each(function(index) {
						var $this = $(this);

						$this.velocity({
							opacity: 1.0,
							translateZ: 0,
							translateY: ['0', '30px']
						}, {
							duration: 500, 
							delay: index * 150,
							easing: Alex.meta.easingInValue
						});
					});    
        
				// Send View of Section 1
				//ga('send', 'event', Alex.meta.currentSection, 'View', '');

				$('.down-arrow').on('click', function(e) {
					e.preventDefault();

					Alex.meta.fullPageHardStop = true;
					$.fn.fullpage.moveSectionDown();
				});

			} else if (Alex.meta.wWidth >= 768 && Alex.meta.isMobile) {
				Alex.meta.currentSection = 'MobileView';
				Alex.loadLazyImages();
				Alex.initFaderNav();
				Alex.handleHash();

				// Animate first text slide in.
				Alex.ui.$textSlides.eq(textIndex)
					.find('.js-animate-this')
					.each(function(index) {
						var $this = $(this);

						$this
							.velocity('finish').velocity({
								opacity: 1.0,
								translateY: ['0', '30px'],
								translateZ: 0
							}, {
								duration: 500, 
								delay: index * 150,
								easing: Alex.meta.easingInValue
							});
					});   
			} else {
				Alex.meta.currentSection = 'MobileView';
        
				// Animate first text slide in.
				Alex.ui.$textSlides
					.eq(textIndex)
					.find('.js-animate-this')
					.each(function(index) {
						var $this = $(this);

						$this.velocity('finish').velocity({
							opacity: 1.0,
							translateY: ['0', '30px'],
							translateZ: 0
						}, {
							duration: 500, 
							delay: index * 150,
							easing: Alex.meta.easingInValue
						});
					});   
			}
		},
		animateSlideText: function(idx) {
			Alex.ui.$imageSlides
				.eq(idx-1)
				.velocity({
					opacity: 0.0,
					translateZ: 0
				}, {
					duration: 500,
					complete: function() {
						Alex.ui.$imageSlides
							.eq(idx)
							.velocity({
								opacity: 1.0,
								translateZ: 0
							}, {
								duration: 250,
								visibility: 'visible'
							});
					}
				});
		},
		loadMobileImages: function() {
			Alex.ui.$imageSlides.each(function() {
				var $this = $(this),
					imgSource = $this.data('src');

				$this.attr('src', imgSource);
			});
		},
		loadLazyImages: function() {
			Alex.ui.$lazyImgs.each(function(index) {
				var $this = $(this),
					imgSource = $this.data('image-src');

				$this
					.attr('src', imgSource);
				$this.velocity({
					opacity: 1.0,
					translateZ: 0
				}, {
					duration: 300,
					visibility: 'visible',
					easing: Alex.easingInValue,
					delay: index * 250
				});
			});
		},
		initAboutPage: function() {
			Alex.meta.currentSection = 'About';
		},
		initFaqPage: function() {
			Alex.meta.currentSection = 'FAQ';
		},
		initInterPage: function() {
			Alex.meta.currentSection = 'Interstitial';
		},
		initFaderNav: function() {
			Alex.ui.$faderLinks.on('click', function(e) {
				e.preventDefault();

				Alex.meta.fullPageHardStop = true;

				var $this = $(this),
					newHash = $this.data('overlay-target') ? $this.data('overlay-target').split('-')[0] : '';

				if ($this.hasClass('close-overlay')) {
					if (!Alex.ui.$headerWrapper.hasClass('overlay-is-active')) {
						window.location.href ='';
					}
				}

				window.location.hash = newHash ? '/' + newHash : '';
			});
		},
		initGATracking: function() {
			Alex.ui.$gaTrackItems.on('click', function() {
				var $this = $(this),
					trackLabel = $this.data('ga-label');

				//ga('send', 'event', 'Section' + Alex.meta.currentSection, 'click', trackLabel);
			});
		}
	};

	$(function () {
		Alex.init();
	});
  
})(jQuery, window, document);
