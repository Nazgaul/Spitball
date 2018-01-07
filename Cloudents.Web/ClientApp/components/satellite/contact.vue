<template v-once>
<general-page header="./img/contact-header.png" icon="./img/contact-icon.png">
    <div class="contact">
        <a href="mailto:support@spitball.co" class="d-block">support@spitball.co</a>
        <span class="d-block">79 Madison Ave, New York, New York, 10016</span>
        <v-layout row class="mt-3 mb-3">
            <v-flex v-for="(bIcon,index) in bottomIcons" :key="index" align-start justify-start class="social-btns mr-4">
                <a :href="bIcon.link">
                    <component class="icons" :is="'svg-' + bIcon.img"></component>
                </a>
            </v-flex>
        </v-layout>
        <div>
            <iframe class="map" src="https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d3022.816148528182!2d-73.98741088437772!3d40.744070843589476!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x89c259a7b392c325%3A0x3e7f464912da083e!2s79+Madison+Ave%2C+New+York%2C+NY+10016%2C+USA!5e0!3m2!1sen!2sil!4v1514983784728" width="608" height="321" frameborder="0" allowfullscreen></iframe>
        </div>
    </div>
</general-page>
</template>
<script>
    import {bottomIcons} from "../home/consts";
    import generalPage from "./generalImage.vue";

    var components = {generalPage};

    for(var t in bottomIcons) {
        const item = bottomIcons[t];
        components["svg-" + item.img] = item.svg;
    }
    export default {
         components: components,
         data() {
             return {
                 bottomIcons
             }
         }
     }
</script>
<style lang="less">
    @import "../../mixin.less";
    
    .wrapper {
        font-size: 18px;
        letter-spacing: -0.2px;
        text-align: left;
        color: #302eb5;
    }
    .icons {
        fill:#3293f6;
       
    }
    .social-btns {
         .flexSameSize()
    }
    .map {
        border: solid 2px #3293f6;
    }
    
</style>

