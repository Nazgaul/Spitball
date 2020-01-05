<template>
        <v-container class="suggestCard-container" >
            <v-layout align-center xs12 sm6 wrap class="suggestCard-sections">
                <div class="suggestCard-texts">
                    <h3 v-language:inner="isMobile ? 'suggestCard_title_mobile' : 'suggestCard_title'"></h3>
                    <h4 v-language:inner="'suggestCard_body'"></h4>
                </div>
                <div class="suggestCard-btn-section">
                    <button class="suggestCard-btn" v-language:inner="'suggestCard_btn'"></button>
                </div>
            </v-layout>
        </v-container>
</template>
<script>
export default {
    computed: {
        isMobile() {
            return this.$vuetify.breakpoint.xsOnly;
        },
    },
}
</script>
<style src="./suggestCard.less" lang="less"></style>