<template>
    <div class="footer">
        <div class="container">
            <div class="footer-content">
                <ul v-if="links" class="footer-list w-list-unstyled">
                    <li  v-for="(link, index) in links" :key="index">
                        <a href="#" class="footer-link">{{link.title}}</a>
                    </li>
                </ul>
                <div class="footer-contact-box">
                    <div class="contact-footer-thin-line"></div>
                    <div class="footer-link">We&#x27;re here to help</div>
                    <div class="footer-link">+52 8830 686</div>
                    <div class="footer-link">support@spitball.co</div>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
import {LanguageService} from '../../services/language/languageService'
    export default {
        name: "Footer",
        data(){
            return{
                links:[
                    {title: LanguageService.getValueByKey('landingPage_footer_links_about'), url: ''},
                    {title: LanguageService.getValueByKey('landingPage_footer_links_feedback'), url: ''},
                    {title: LanguageService.getValueByKey('landingPage_footer_links_terms'), url: ''},
                    {title: LanguageService.getValueByKey('landingPage_footer_links_privacy'), url: ''},
                    {title: LanguageService.getValueByKey('landingPage_footer_links_faq'), url: ''},
                    {title: LanguageService.getValueByKey('landingPage_footer_links_ambassadors'), url: ''},
                    {title: LanguageService.getValueByKey('landingPage_footer_links_blog'), url: ''},
                    {title: LanguageService.getValueByKey('landingPage_footer_links_contact'), url: ''},
                    {title: LanguageService.getValueByKey('landingPage_footer_links_partners'), url: ''},
                    {title: LanguageService.getValueByKey('landingPage_footer_links_pr'), url: ''},
                ]
            }
        },
    }
</script>

<style>

</style>