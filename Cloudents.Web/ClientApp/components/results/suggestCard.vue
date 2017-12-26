<template>
    <button type="button" class="suggest">
        <v-container>
            <v-layout class="wrapper" row justify-space-between>
                    <div class="logo">Spitball</div>
                    <div class="action">Show me</div>
            </v-layout>
            <div class="text">
                I found some sweet {{name}} for you.  You can thank me later:)
            </div>
        </v-container>
    </button>
</template>
pl
ip
<script>
    import logo from "./svg/sb-logo.svg";
    export default {
        components: { logo },

        props: {
            name: { type: String }
        }
    }
</script>
<style src="./suggestCard.less" lang="less" scoped></style>