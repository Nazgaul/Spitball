<template>
    <v-layout class="cta-section mt-5 mb-5">
        <v-flex xs12>
            <v-layout align-center justify-center column>
                <v-flex xs12 sm4 md4 class="text-xs-center text-sm-center text-md-center">
                    <div class="cta-title mb-4 "v-language:inner>profile_cta_section_title
                    </div>
                </v-flex>
                <v-flex class="text-xs-center text-sm-center text-md-center mt-3 mb-">
                    <contactBtn></contactBtn>
                </v-flex>
            </v-layout>
        </v-flex>
    </v-layout>
</template>

<script>
    import contactBtn from '../../profileHelpers/contactBtn/contactBtn.vue';

    export default {
        name: "ctaBlock",
        components: {contactBtn}
    }
</script>

<style lang="less">
    @import '../../../../styles/mixin.less';

    .cta-section {
        .cta-title {
            color: @profileTextColor;
            font-size: 20px;
            font-weight: bold;
            line-height: 0.85;
        }
    }


</style>