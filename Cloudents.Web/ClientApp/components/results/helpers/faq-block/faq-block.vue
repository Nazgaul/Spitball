<template>
    <v-flex class="right-sidebar" :class="{'bannerActive':getBannerParams}">
        <v-flex xs12 class="card-block">
            <marketing-box/>
        </v-flex>
        <v-flex v-if="!!suggestList[name]" xs12 class="card-block mt-4">
        </v-flex>
    </v-flex>
</template>


<script>
    import {suggestList} from "./../../consts"
    import {mapGetters, mapActions } from 'vuex';
    import marketingBox from "../../../helpers/marketingBox/marketingBox.vue";
    export default {
        components:{
            marketingBox,
        },
        data() {
            return {

                faqList: null,
                suggestList,
                showDialogLogin: false
            }
        },
        props: {
            name: {},
            text: {},
            isAsk: Boolean,
            isNotes: Boolean
        },
        computed: {
            ...mapGetters(['accountUser','loginDialogState','getBannerParams'])
        },
        methods:{
            ...mapActions(["updateLoginDialogState"]),
        },
    }
</script>

<style src="./faq-block.less" lang="less"></style>