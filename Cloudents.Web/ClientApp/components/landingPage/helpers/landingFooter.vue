<template>
    <footer class="footer">
        <v-card class="footer-card">
            <white-logo></white-logo>
        </v-card>
        <v-card class="footer-card social-card">
                <div  class="social-link"><v-icon>sbf-social-medium</v-icon></div>
                <div  class="social-link"><v-icon>sbf-social-linkedin</v-icon></div>
                <div  class="social-link"><v-icon>sbf-social-facebook</v-icon></div>
                <div  class="social-link"><v-icon>sbf-social-youtube</v-icon></div>
                <div  class="social-link"><v-icon>sbf-social-telegram</v-icon></div>
                <!--<div  class="social-link"><v-icon>sbf-social-twitter</v-icon></div>-->

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
    export default {
        components: {whiteLogo},
        name: "landingFooter",
        data() {
            return {
                footerLinks: [
                    {
                        id: '1',
                        linkText: 'About Spitball',
                        linkValue: '',
                        linkTarget: ''
                    },
                    {
                        id: '2',
                        linkText: 'Ambassadors',
                        linkValue: '',
                        linkTarget: ''
                    },
                    {
                        id: '3',
                        linkText: 'Feedback',
                        linkValue: '',
                        linkTarget: ''
                    },
                    {
                        id: '4',
                        linkText: 'Terms & Conditions',
                        linkValue: '',
                        linkTarget: ''
                    },
                    {
                        id: '5',
                        linkText: 'Partners',
                        linkValue: '',
                        linkTarget: ''
                    },
                    {
                        id: '6',
                        linkText: 'FAQ',
                        linkValue: '',
                        linkTarget: ''
                    },
                    {
                        id: '7',
                        linkText: 'Privacy Policy\n',
                        linkValue: '',
                        linkTarget: ''
                    },
                    {
                        id: '8',
                        linkText: 'PR',
                        linkValue: '',
                        linkTarget: ''
                    },
                    {
                        id: '9',
                        linkText: 'Contact Us',
                        linkValue: '',
                        linkTarget: ''
                    },
                    {
                        id: '10',
                        linkText: 'Blog',
                        linkValue: '',
                        linkTarget: ''
                    },


                ]
            }
        },
    }
</script>

<style scoped lang="less">
    @import "../../../styles/mixin.less";

    @footerLinksColor: rgba(255, 255, 255, 0.87);
    @footerBackground: rgba(3, 4, 21, 0.72);
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
            }
            &.social-card{
                display: flex;
                flex-direction: row;
                align-items: center;
                justify-content: space-around;
                height: 80px;
                border-top: 1px solid #979797;
                border-bottom: 1px solid #979797;;
                .social-link{
                    width: 50px;
                   i{
                       color: @color-white;
                       font-size: 22px;
                   }
                }
            }
        }
        .footer-links {
            display: flex;
            flex-basis: 50%;
            align-items: center;
            justify-content: center;
            padding: 16px;
            a{
                font-size: 13px;
                color: @footerLinksColor;
                text-decoration: none;
                &:hover{
                    color: fade(@footerLinksColor, 100%);
                }
            }

        }
    }


</style>