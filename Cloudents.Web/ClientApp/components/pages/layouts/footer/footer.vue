<template>
    <div class="footer">
        <v-layout align-center justify-space-around class="footer-warp">
            <ul v-if="links" class="footer-wrap-list w-list-unstyled">
                <li  v-for="(link, index) in links" :key="index">
                    <a :href="link.url" v-if="isShowBlog(link)" class="footer-link">{{link.title}}</a>
                </li>
            </ul>
            <div class="footer-warp-divider mt-6"></div>
            <div class="footer-contact-box">
                <div class="tutor-list-footer-logo">
                    <logoComponent></logoComponent>
                </div>
                
                <div class="footer-contact-box-icons">
                    <a v-for="(sm, index) in socialMedias" :key="index" :href="sm.url" target="_blank"><v-icon>{{sm.icon}}</v-icon></a>
                </div>
            </div>
        </v-layout>
        <div v-if="isTutorList" class="tutorList_footer">
            <h1 class="tutorList_footer_txt" v-if="!courseTerm" v-language:inner="'tutorListLanding_footer_h1'"/>
            <h1 class="tutorList_footer_txt" v-else v-html="$Ph('tutorListLanding_footer_h1_course',[courseTerm,courseTerm])"/>
    
            <h2 class="tutorList_footer_txt" v-if="!courseTerm" v-language:inner="'tutorListLanding_footer_h2'"/>
            <h2 class="tutorList_footer_txt" v-else v-html="$Ph('tutorListLanding_footer_h2_course',[courseTerm,courseTerm])"/>

            <h3 class="tutorList_footer_txt" v-if="!courseTerm" v-language:inner="'tutorListLanding_footer_h3'"/>
            <h3 class="tutorList_footer_txt" v-else v-html="$Ph('tutorListLanding_footer_h3_course',courseTerm)"/>
        </div>
    </div>
</template>

<script>
import {LanguageService} from '../../../../services/language/languageService';
import satelliteService from '../../../../services/satelliteService';
import logoComponent from '../../../app/logo/logo.vue';

    export default {
        name: "Footer",
        components: {
            logoComponent,
        },
        data(){
            return{
                courseTerm:'',
                socialMedias: satelliteService.getSocialMedias(),
                links:[
                    {   
                        name:'about',
                        title: LanguageService.getValueByKey('tutorListLanding_footer_links_about'), 
                        url: satelliteService.getSatelliteUrlByName('about')
                    },
                    {
                        name:'feedback',
                        title: LanguageService.getValueByKey('tutorListLanding_footer_links_feedback'),
                        url: satelliteService.getSatelliteUrlByName('feedback')
                    },
                    {
                        name:'terms',
                        title: LanguageService.getValueByKey('tutorListLanding_footer_links_terms'),
                        url: satelliteService.getSatelliteUrlByName('terms')
                    },
                    {
                        name:'privacy',
                        title: LanguageService.getValueByKey('tutorListLanding_footer_links_privacy'),
                        url: satelliteService.getSatelliteUrlByName('privacy')
                    },
                    {
                        name:'faq',
                        title: LanguageService.getValueByKey('tutorListLanding_footer_links_faq'),
                        url: satelliteService.getSatelliteUrlByName('faq')
                    },
                    {
                        name:'contact',
                        title: LanguageService.getValueByKey('tutorListLanding_footer_links_contact'),
                        url: satelliteService.getSatelliteUrlByName('contact')
                    },
                    {
                        name:'blog',
                        title: LanguageService.getValueByKey('tutorListLanding_footer_links_blog'),
                        url: satelliteService.getSatelliteUrlByName('blog')
                    }
                ],
            }
        },
        computed: {
            isTutorList(){
                return this.$route.name === 'tutorLandingPage';
            },
        },
        methods: {
            isShowBlog(link){
                if(link.name == 'blog' && global.siteName =='frymo'){
                    return false;
                }else{
                    return true;
                }
            },
            footerLinksRoute(link) {
                if(link === 'blog') {
                    window.open('https://medium.com/@spitballstudy')
                }else if(link ===  'feedback') {
                    Intercom('showNewMessage')
                }else {
                    this.$router.push({name: link});
                }
            },
            checkParams(){
                if(this.$route.name === 'tutorLandingPage'){
                    if(!!this.$route.params && this.$route.params.course){
                        this.courseTerm = this.$route.params.course
                    }
                }
            }
        },
        mounted() {
            this.checkParams()
        },
        watch:{
            '$route'(){
                this.$nextTick(()=>{
                    this.checkParams()
                })
            },
        }
    }
</script>

<style lang="less">
@import "../../../../styles/mixin.less";

.footer {
  background-color: #f1f1f4;
  padding-top: 30px;
  padding-bottom: 20px;
  @media (max-width: @screen-sm) {
    padding-top: 20px;
  }
  @media (max-width: @screen-xs) {
        padding-bottom: 76px;
        padding-top: 0;
  }
  .tutorList_footer{
      margin-top: 14px;
    @media (max-width: @screen-xs) {
        padding: 0 20px;
    }
      .tutorList_footer_txt{
        opacity: 0.3;
        font-weight: normal;
        font-size: 12px;
        text-align: center;
      }
  }
 .footer-warp {
    max-width: 1200px;
    margin: 0 auto;
    height: inherit;
    font-size: 14px;
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
        @media (max-width: @screen-xs) {
            margin-top: 20px;
        }
        li {
            margin-left: 50px;
            text-align: left;
            @media (max-width: @screen-sm) {
                margin: 10px 0;
                text-align: center;
            }
          .footer-link {
              color: @global-purple;
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
        @media (max-width: @screen-xs) { 
            margin-top: 8px !important;
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
                color: @global-purple;
                cursor: pointer;
                i{
                    color: @global-purple;
                }
            }
        }
    }
  }
  .tutor-list-footer-logo {
        div{
          svg {
            vertical-align: -webkit-baseline-middle;
            fill: @global-purple;
            &.frymo-logo{
              fill: @global-purple;
            }
            
          }
        } 
      }
  .tutor-list-footer-logo {
        div{
          svg {
            vertical-align: -webkit-baseline-middle;
            fill: @global-purple;
            &.frymo-logo{
              fill: @global-purple;
            }
            
          }
        } 
      }
}

</style>
