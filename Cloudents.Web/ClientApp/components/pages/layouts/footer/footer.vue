<template>
    <div class="footer">
        <v-layout align-center class="footer-warp">
            <ul v-if="links" class="footer-wrap-list w-list-unstyled">
                <li  v-for="(link, index) in links" :key="index">
                    <a :href="link.url" v-if="isShowBlog(link)" class="footer-link">{{link.title}}</a>
                </li>
            </ul>
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
  @media (max-width: @screen-xs) {
    padding-bottom: 20px;
  }
  @media (max-width: 941px) { // flex-wrap is breaking in 941
    padding-top: 20px;
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
    flex-wrap: wrap;
    justify-content: space-around;
    @media (max-width: @screen-sm) {
        justify-content: start;
        margin: 0 50px;
    }
    @media (max-width: @screen-xs) {
        flex-wrap: nowrap;
        justify-content: center;
        flex-direction: column;
        margin: 0;
    }
    .footer-wrap-list {
        padding: 0;
        list-style: none;
        line-height: 40px;
        @media screen and (min-width: @screen-xs) {
            text-align: center;
            -webkit-column-count: 3;
            -moz-column-count: 3;
            column-count: 3;
            column-gap: 60px;
            -webkit-column-gap: 60px;
            -moz-column-gap: 60px;
        }
        @media (max-width: @screen-xs) {
            margin: 0 50px;
        }
        li {
            text-align: left;
            @media (max-width: @screen-xs) {
                text-align: center;
            }
          .footer-link {
            cursor: pointer;
            color: @global-purple;
            font-size: 16px;
        }
      }
    }
    .footer-contact-box {
        line-height: 80px;
        @media (max-width: @screen-xs) { 
            padding-right: 0;
        }
        :first-child {
            text-align: left;
            @media (max-width: @screen-xs) { 
                text-align: center;
            }
        }
        .footer-contact-box-icons {
            a {
                margin: 0 10px;
                color: @global-purple;
                cursor: pointer;
                i {
                    color: @global-purple;
                }
            }
        }
    }
  }
  .tutor-list-footer-logo {
        div{
          svg {
            width: auto;
            height: auto;
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
