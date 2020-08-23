<template>
    <v-dialog :value="true" content-class="unSupportedFeature text-center px-6" fullscreen persistent>
        <v-icon size="13" class="closeIcon" @click="$router.push('/')" color="grey">{{$vuetify.icons.values.close}}</v-icon>
        <unSupportSvg class="svgIcon mt-5" />
        <div class="unSupportedText my-4 px-5" v-t="'unsupported_feature'"></div>
        <div class="unSupportedText" v-t="'unsupported_feature2'"></div>
    </v-dialog>
</template>

<script>
import unSupportSvg from './unSupported.svg'
export default {
    name: 'unSupportedFeature',
    components: {
        unSupportSvg
    }
}
</script>

<style lang="less">
.unSupportedFeature {
    background: #fff;
    .closeIcon {
        width: auto;
        position: absolute;
        right: 8px;
        top: 8px;
    }
    .svgIcon {
        height: auto;
        width: 120px;
    }
    .unSupportedText {
        font-size: 20px;
        line-height: 1.6;
        text-align: center;
        color: #43425d;
    }
}
</style>