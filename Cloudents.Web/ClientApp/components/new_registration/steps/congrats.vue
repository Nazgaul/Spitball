<template>
    <div class="step-account">
        <step-template>
            <div slot="step-text" class="text-block-slot" v-if="isMobile">
                <div class="text-wrap-top">
                    <p class="text-block-sub-title" v-html="meta.heading"></p>
                    <p class="text-block-sub-title"
                      v-html="meta.subheading"></p>
                </div>
            </div>
            <div slot="step-data" class="limited-width done">
                <h2 v-if="!isMobile" class="congrats-heading"
                    v-html="meta.heading">{{meta.heading}}</h2>
                <h2 v-if="!isMobile" class="congrats-heading">{{meta.subheading}}</h2>
                <img v-if="!isMobile" class="money-done-img" :src="require(`../img/money-done.png`)"/>
                <p class="congrats" v-html="meta.text">{{meta.text}}</p>

                <v-btn class="continue-btn submit-code"
                       value="congrats"
                       :loading="loading"
                       @click="finishRegistration"><span v-language:inner>login_lets_start</span>
                </v-btn>
            </div>
            <img slot="step-image" :src="require(`../img/done.png`)"/>
        </step-template>
    </div>

</template>

<script>
    import stepTemplate from '../helpers/stepTemplate.vue'
    import analyticsService from '../../../services/analytics.service';
    import SbInput from "../../question/helpers/sbInput/sbInput.vue";
    const isIl = global.country.toLowerCase() === 'il';
    const defaultSubmitRoute = isIl ? {path: '/note'} : {path: '/ask'};
    import { mapActions } from 'vuex';
    export default {

        name: "step_6",
        components: {stepTemplate, SbInput},

        data() {
            return {
                confirmationCode: '',
                loading: false,
            }
        },
        props: {
            isMobile: {
                type: Boolean,
                default: false
            },
            meta: {},
            lastActiveRoute: '',
            userEmail: "",
            toUrl: {}
        },
        methods: {
            ...mapActions(['changeSelectUniState', 'updateSelectForTheFirstTime']),
            finishRegistration() {
                this.loading = true;
                analyticsService.sb_unitedEvent('Registration', 'Congrats');
                let url = this.toUrl || defaultSubmitRoute;
                window.isAuth = true;
                this.loading = false;

                let self = this;
                //show selectUni interface
                self.updateSelectForTheFirstTime(true);
               
               
                this.$router.push({name:'uniselect'})
                // this.$router.push({path: `${url.path }`});
            },
        },
    }
</script>

<style scoped>

</style>