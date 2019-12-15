<template>
	<v-system-bar v-if="getBannerSatus" class="globalBanner" height="70" app fixed>
		<div class="banner_wrapper">
			<div class="globalBanner_container">
				<div class="globalBanner_title" v-if="titleText">       
					<h1 class="globalBanner_title_get" v-html="titleText"/>
					<h2 class="globalBanner_title_sub" v-html="subTitleText"/>
					<span class="globalBanner_title_sub banner_code" v-if="$vuetify.breakpoint.xsOnly">
						<span v-language:inner="'banner_code'"/>
						<span class="globalBanner_title_code" v-html="coupon"/>
					</span>
				</div>
				<div class="globalBanner_coupon mt-1" v-if="!$vuetify.breakpoint.xsOnly && !!getBannerParams">
					<img class="globalBanner_coupon_img" src="./images/b.png" alt="">
					<span v-language:inner="'banner_code'"/>
					<span class="globalBanner_coupon_code" v-html="coupon"/>
				</div>
				<div class="globalBanner_timer" v-if="$vuetify.breakpoint.mdAndUp && !!getBannerParams">
					<span class="globalBanner_timer_title" v-language:inner="'banner_offer'"/>
					<v-layout class="globalBanner_timer_container mt-1">
						<v-flex xs2 class="globalBanner_timer_box">
							<h1 class="globalBanner_timer_box_time" v-html="time.days"/>
							<span class="globalBanner_timer_box_text" v-language:inner="'banner_days'"/>
						</v-flex>
						<v-flex class="globalBanner_timer_divider">:</v-flex>
						<v-flex xs2 class="globalBanner_timer_box">
							<h1 class="globalBanner_timer_box_time" v-html="time.hours"/>
							<span class="globalBanner_timer_box_text" v-language:inner="'banner_hours'"/>
						</v-flex>
						<v-flex class="globalBanner_timer_divider">:</v-flex>
						<v-flex xs2 class="globalBanner_timer_box">
							<h1 class="globalBanner_timer_box_time" v-html="time.minutes"/>
							<span class="globalBanner_timer_box_text" v-language:inner="'banner_minutes'"/>
						</v-flex>
						<v-flex class="globalBanner_timer_divider">:</v-flex>
						<v-flex xs2 class="globalBanner_timer_box">
							<h1 class="globalBanner_timer_box_time" v-html="time.seconds"/>
							<span class="globalBanner_timer_box_text"  v-language:inner="'banner_seconds'"/>
						</v-flex>
					</v-layout>
				</div>
			</div>  
			<v-btn flat icon class="banner_closeBtn" @click="updateBannerSatus(false)">
				<v-icon class="close_banner" v-html="'sbf-close'"/>
			</v-btn>
		</div>
	</v-system-bar>
</template>

<script>
import { mapGetters, mapActions } from 'vuex';
export default {
	data() {
		return {
			interVal:null,
			time:{
				days:'00',
				hours:'00',
				minutes:'00',
				seconds:'00'
			}
		}
	},
	computed: {
		...mapGetters(['getBannerSatus','getBannerParams']),
		params(){
			if(!!this.getBannerParams){
				this.interVal = setInterval(this.getNow, 200);
			}
		},
		coupon(){
			if(!!this.getBannerParams){
				return this.getBannerParams.coupon;
			}
		},
		titleText(){
			if(!!this.getBannerParams){
				return this.getBannerParams.title;
			}
		},
		subTitleText(){
			if(!!this.getBannerParams){
				return this.getBannerParams.subTitle;
			}
		}
	},
		watch:{
			params(val){
				// debugger
			}
	},
	methods: {
		...mapActions(['updateBannerSatus']),
		getNow() {
			let countDownDate = new Date(this.getBannerParams.expirationDate).getTime();
			let now = new Date();
			let distance = countDownDate - now;

			this.time.days = Math.floor(distance / (1000 * 60 * 60 * 24)).toLocaleString('en-US', {minimumIntegerDigits: 2});
			this.time.hours = Math.floor((distance % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60)).toLocaleString('en-US', {minimumIntegerDigits: 2});
			this.time.minutes = Math.floor((distance % (1000 * 60 * 60)) / (1000 * 60)).toLocaleString('en-US', {minimumIntegerDigits: 2});
			this.time.seconds = Math.floor((distance % (1000 * 60)) / 1000).toLocaleString('en-US', {minimumIntegerDigits: 2});
			if (distance < 0) {
				clearInterval(this.interVal);
				this.updateBannerSatus(false)
			}
		}
	},
}
</script>

<style lang="less">
@import '../../../../styles/mixin.less';
.globalBanner{
    z-index: 20;
    background-image: linear-gradient(to bottom, #7072fb -20%, #1a2b87 96%);
    .banner_wrapper{
        position: relative;
        width: 100%;
        .banner_closeBtn{
            position: absolute;
            top: -10px;
            right: -16px;
            .close_banner{
                color: white !important;
                opacity: 0.57;
                font-size: 12px
            }
        }
    .globalBanner_container{
        max-width: 860px;
        width: 100%;
        margin: 0 auto;
        display: flex;
        justify-content: space-between;
        @media (max-width: @screen-md) {
            justify-content: space-evenly;
        }
        @media (max-width: @screen-xs) {
            justify-content: center;
        }
        align-items: center;
        color: #ffffff; 
        .globalBanner_title{
        @media (max-width: @screen-xs) {
            text-align: center;
        }
            .globalBanner_title_get{
                font-size: 22px;
                font-weight: bold;
                color: #6ff8f0;
                line-height: 1;
                @media (max-width: @screen-xs) {
                    font-size: 20px;
                }
            }
            .globalBanner_title_sub{
                    margin-top: 2px;
                font-size: 18px;
                font-weight: normal;
                font-stretch: normal;
                font-style: normal;
                line-height: 1.25;
                letter-spacing: normal;
                @media (max-width: @screen-xs) {
                        margin-top: 0;
                    font-size: 14px;
                }
                &.banner_code{
                    font-size: 12px;
                    .globalBanner_title_code{
                        font-weight: 600;
                        color: #6ff8f0; 
                    }
                }
            }
                
        }
        .globalBanner_coupon{
            position: relative;
            border-radius: 4px;
            border: dashed 1.5px #ffffff;
            font-size: 20px;
            font-weight: 600;
            font-stretch: normal;
            font-style: normal;
            line-height: normal;
            letter-spacing: normal;
            padding: 8px 18px;
            .globalBanner_coupon_img{
                transform: scaleX(1) /*rtl:append:scaleX(-1)*/;
                position: absolute;
                top: -15px;
                left: 8px;
            }
            .globalBanner_coupon_code{
                color: #6ff8f0;
            }
        }
        .globalBanner_timer{
            text-align: center;
            .globalBanner_timer_divider{
                opacity: 0.39;
                font-size: 30px;
                font-weight: normal;
                font-stretch: normal;
                font-style: normal;
                line-height: 1;
                letter-spacing: normal;
                margin: 0 8px;
            }
            .globalBanner_timer_title{
                font-size: 14px;
                font-weight: normal;
                font-stretch: normal;
                font-style: normal;
                line-height: normal;
                letter-spacing: normal;
            }
            .globalBanner_timer_container{
                /*rtl:ignore*/
                direction: ltr;
                /*rtl:ignore*/
                .globalBanner_timer_box{
                    text-align: center;
                    .globalBanner_timer_box_time{
                        font-size: 26px;
                        font-weight: normal;
                        font-stretch: normal;
                        font-style: normal;
                        line-height: 0.9;
                        letter-spacing: normal;
                    }
                    .globalBanner_timer_box_text{
                        font-size: 9px;
                        font-weight: 600;
                        font-stretch: normal;
                        font-style: normal;
                        line-height: normal;
                        letter-spacing: normal;
                    }
                }
            }
        }
    }
        }
}
</style>