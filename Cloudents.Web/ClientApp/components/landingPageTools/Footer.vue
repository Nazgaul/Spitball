<template>
    <div class="footer">
        <v-layout row align-center justify-space-around class="footer-warp">
            <ul v-if="links" class="footer-wrap-list w-list-unstyled">
                <li  v-for="(link, index) in links" :key="index">
                    <a :href="link.url" class="footer-link">{{link.title}}</a>
                </li>
            </ul>
            <div class="footer-warp-divider mt-4"></div>
            <div class="footer-contact-box">
                <div>
                    <LOGO></LOGO>
                </div>
                <div class="footer-contact-box-icons">
                    <a href="https://medium.com/@spitballstudy" target="_blank"><v-icon>sbf-social-medium-small</v-icon></a>
                    <a href="https://linkedin.com/company/spitball" target="_blank"><v-icon>sbf-social-linkedin</v-icon></a>
                    <a href="https://www.facebook.com/spitballstudy/" target="_blank"><v-icon>sbf-social-facebook</v-icon></a>
                    <a href="https://www.youtube.com/channel/UCamYabfxHUP3A9EFt1p94Lg/" target="_blank"><v-icon>sbf-social-youtube</v-icon></a>
                    <a href="https://t.me/Spitball" target="_blank"><v-icon>sbf-social-telegram</v-icon></a>
                    <a href="https://twitter.com/spitballstudy" target="_blank"><v-icon>sbf-social_twitter</v-icon></a>
                </div>
            </div>
        </v-layout>
    </div>
</template>

<script>
import {LanguageService} from '../../services/language/languageService';
import LOGO from './sp-logo.svg';
import satelliteService from '../../services/satelliteService';

    export default {
        name: "Footer",
        components: {
            LOGO,
        },
        data(){
            return{
                links:[
                    {
                        title: LanguageService.getValueByKey('tutorListLanding_footer_links_about'), 
                        url: satelliteService.getSatelliteUrlByName('about')
                    },
                    {
                        title: LanguageService.getValueByKey('tutorListLanding_footer_links_feedback'),
                        url: satelliteService.getSatelliteUrlByName('feedback')
                    },
                    {
                        title: LanguageService.getValueByKey('tutorListLanding_footer_links_terms'),
                        url: satelliteService.getSatelliteUrlByName('terms')
                    },
                    {
                        title: LanguageService.getValueByKey('tutorListLanding_footer_links_privacy'),
                        url: satelliteService.getSatelliteUrlByName('privacy')
                    },
                    {
                        title: LanguageService.getValueByKey('tutorListLanding_footer_links_faq'),
                        url: satelliteService.getSatelliteUrlByName('faq')
                    },
                    {
                        title: LanguageService.getValueByKey('tutorListLanding_footer_links_contact'),
                        url: satelliteService.getSatelliteUrlByName('contact')
                    },
                ]
            }
        },
        methods: {
            footerLinksRoute(link) {
                if(link === 'blog') {
                    window.open('https://medium.com/@spitballstudy')
                }else if(link ===  'feedback') {
                    Intercom('showNewMessage')
                }else {
                    this.$router.push({name: link});
                }
            }
        }
    }
</script>

<style lang="less">
@import "../../styles/mixin.less";
.footer {
  height: 400px;
  background-color: #1b2441;
  color: #fff;
  @media (max-width: @screen-sm) {
      padding: 80px 0;
      height: auto;
  }
 .footer-warp {
    max-width: 1200px;
    margin: 0 auto;
    height: inherit;
    @media (max-width: @screen-sm) {
        flex-direction: column-reverse;
    }
    .footer-wrap-list {
        cursor: pointer;
        padding: 0;
        list-style: none;
        @media (min-width: 768px){
            text-align: center;
            -webkit-column-count: 3;
            -moz-column-count: 3;
            column-count: 3;
            column-gap: 60px;
            -webkit-column-gap: 60px;
            -moz-column-gap: 60px;
            line-height: 40px;
        }
        @media (max-width: @screen-sm) {
            margin-top: 50px;
        }
        li {
            margin-left: 50px;
            text-align: left;
            @media (max-width: @screen-sm) {
                margin: 10px 0;
                text-align: center;
            }
          .footer-link {
              color: #fff;
              font-size: 16px;
        }
      }
    }
    .footer-warp-divider {
        width: 75px;
        height: 1px;
        border: solid 1px #979797;
        display: none;
        @media (max-width: @screen-sm) { 
            display: block;
        }
     }
    .footer-contact-box {
        line-height: 80px;
        :first-child {
            text-align: center;
        }
        .footer-contact-box-icons {
            i{
                margin: 0 10px;
                color: #FFF;
                cursor: pointer;
            }
            
        }
    }
  }
}

</style>
