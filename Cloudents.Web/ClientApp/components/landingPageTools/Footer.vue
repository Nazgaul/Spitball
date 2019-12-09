<template>
    <div class="footer">
        <v-layout align-center justify-space-around class="footer-warp">
            <ul v-if="links" class="footer-wrap-list w-list-unstyled">
                <li  v-for="(link, index) in links" :key="index">
                    <a :href="link.url" class="footer-link">{{link.title}}</a>
                </li>
            </ul>
            <div class="footer-warp-divider mt-4"></div>
            <div class="footer-contact-box">
                <div class="tutor-list-footer-logo">
                    <logoComponent></logoComponent>
                </div>
                
                <div class="footer-contact-box-icons">
                    <a  v-for="(sm, index) in socialMedias" :key="index" :href="sm.url" target="_blank"><v-icon>{{sm.icon}}</v-icon></a>
                </div>
            </div>
        </v-layout>
    </div>
</template>

<script>
import {LanguageService} from '../../services/language/languageService';
import LOGO from './sp-logo.svg';
import satelliteService from '../../services/satelliteService';
import logoComponent from '../app/logo/logo.vue';

    export default {
        name: "Footer",
        components: {
            logoComponent,
        },
        data(){
            return{
                socialMedias: satelliteService.getSocialMedias(),
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
  background-color: #202431;
  color: #fff;
  @media (max-width: @screen-sm) {
      padding: 20px 0;
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
            a {
                margin: 0 10px;
                color: #FFF;
                cursor: pointer;
                i{
                    color: white;
                }
            }
        }
    }
  }
  .tutor-list-footer-logo {
        div{
          svg {
            vertical-align: -webkit-baseline-middle;
            fill: #FFF;
            &.frymo-logo{
              fill: #FFF;
            }
            
          }
        } 
      }
  .tutor-list-footer-logo {
        div{
          svg {
            vertical-align: -webkit-baseline-middle;
            fill: #FFF;
            &.frymo-logo{
              fill: #FFF;
            }
            
          }
        } 
      }
}

</style>
