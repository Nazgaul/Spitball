<template>
    <div class="footer">
        <div class="container">
            <div class="footer-content">
                <ul class="footer-list w-list-unstyled">
                    <li v-for="link in links">
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
    export default {
        name: "Footer",
        data(){
            return{
                links:[
                    {title: 'About Us', url: ''},
                    {title: 'Careers', url: ''},
                    {title: 'Blog', url: ''},
                    {title: 'Subject answers', url: ''},
                    {title: 'Become a tutor', url: ''},
                    {title: 'Schools', url: ''},
                ]
            }
        }
    }
</script>

<style>

</style>