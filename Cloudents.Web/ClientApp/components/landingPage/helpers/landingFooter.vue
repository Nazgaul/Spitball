<template>
    <footer class="footer">
        <v-card class="footer-card">
            <white-logo></white-logo>
        </v-card>
        <v-card class="footer-card social-card">
            <a href="https://medium.com/@spitballstudy" class="social-link" target="_blank">
                <v-icon>sbf-social-medium</v-icon>
            </a>
            <a href="https://www.linkedin.com/company/spitball/" class="social-link" target="_blank">
                <v-icon>sbf-social-linkedin</v-icon>
            </a>
            <a href="https://www.facebook.com/spitballstudy/" target="_blank" class="social-link">
                <v-icon>sbf-social-facebook</v-icon>
            </a>
            <a href="https://www.youtube.com/channel/UC9qGHTJixreP2bbwugmSi2Q?view_as=subscriber" class="social-link"
               target="_blank">
                <v-icon>sbf-social-youtube</v-icon>
            </a>
            <a href="https://t.me/Spitball" class="social-link" target="_blank">
                <v-icon>sbf-social-telegram</v-icon>
            </a>
            <a href="https://twitter.com/SpitballStudy" class="social-link" target="_blank">
                <v-icon>sbf-social_twitter</v-icon>
            </a>

        </v-card>
        <v-card class="footer-card links-card">
            <div class="footer-links" v-for="(singleLink, index) in footerLinks" :key="singleLink.id">
                <a :href="singleLink.linkValue" class="footer-link">{{singleLink.linkText}}</a>
            </div>
        </v-card>
    </footer>
</template>

<script>
    import whiteLogo from "../images/footerLogo.svg"
    import { LanguageService } from "../../../services/language/languageService";

    export default {
        components: {whiteLogo},
        name: "landingFooter",
        data() {
            return {
                footerLinks: [
                    {
                        id: 0,
                        linkText: LanguageService.getValueByKey("landingPage_footer_about"),
                        linkValue: '/about',
                        linkTarget: '_blank'
                    },
                    {
                        id: 1,
                        linkText: LanguageService.getValueByKey("landingPage_footer_ambassador"),
                        linkValue: '/reps',
                        linkTarget: '_blank'
                    },
                    {
                        id: 2,
                        linkText: LanguageService.getValueByKey("landingPage_footer_feedback"),
                        linkValue: '/faq',
                        linkTarget: '_blank'
                    },
                    {
                        id: 3,
                        linkText: LanguageService.getValueByKey("landingPage_footer_partners"),
                        linkValue: '/partners',
                        linkTarget: '_blank'
                    },
                    {
                        id: 4,
                        linkText: LanguageService.getValueByKey("landingPage_footer_terms"),
                        linkValue: '/terms',
                        linkTarget: '_blank'
                    },
                    {
                        id: 5,
                        linkText: LanguageService.getValueByKey("landingPage_footer_faq"),
                        linkValue: '/faq',
                        linkTarget: '_blank'
                    },
                    {
                        id: 6,
                        linkText: LanguageService.getValueByKey("landingPage_footer_privacy"),
                        linkValue: '/privacy',
                        linkTarget: '_blank'
                    },
                    {
                        id: 7,
                        linkText: LanguageService.getValueByKey("landingPage_footer_pr"),
                        linkValue: '/partners',
                        linkTarget: '_blank'
                    },
                    {
                        id: 8,
                        linkText: LanguageService.getValueByKey("landingPage_footer_contact"),
                        linkValue: '/contact',
                        linkTarget: '_blank'
                    },
                    {
                        id: 9,
                        linkText: LanguageService.getValueByKey("landingPage_footer_blog"),
                        linkValue: '/blog',
                        linkTarget: '_blank'
                    },


                ]
            }
        },
    }
</script>

<style scoped lang="less">
    @import "../../../styles/mixin.less";

    @footerLinksColor: rgba(255, 255, 255, 0.87);
    @footerBackground: #29313F;
    .footer {
        display: flex;
        flex-direction: column;
        align-items: center;
        text-align: center;
        color: @textColor;
        background-color: @footerBackground;
        padding: 60px 0;
        @media (max-width: @screen-xs) {
            .footer {
                padding: 80px 15px;
            }
        }
        .footer-card {
            flex-grow: 1;
            box-shadow: none;
            background-color: transparent;
            &.links-card {
                display: flex;
                flex-direction: row;
                flex-wrap: wrap;
                justify-content: center;
                @media (max-width: @screen-xs) {
                   flex-direction: column;
                }
            }
            &.social-card {
                display: flex;
                flex-direction: row;
                align-items: center;
                justify-content: space-around;
                height: 80px;
                border-top: 1px solid #979797;
                border-bottom: 1px solid #979797;
                margin: 48px 0;
                .social-link {
                    width: 50px;
                    margin-right: 46px;
                    @media (max-width: @screen-xs) {
                        margin-right: 8px;
                    }
                    i {
                        color: @color-white;
                        font-size: 22px;
                    }
                }
            }
        }
        .footer-links {
            display: flex;
            flex-basis: 36%;
            align-items: center;
            justify-content: center;
            padding: 16px 16px 8px 16px;
            a {
                display: inline-block;
                text-align: left;
                width: 50%;
                font-size: 13px;
                color: @footerLinksColor;
                text-decoration: none;
                @media (max-width: @screen-xs) {
                   width: 100%;
                    text-align: center;
                }
                &:hover {
                    color: fade(@footerLinksColor, 100%);
                }
            }

        }
    }


</style>