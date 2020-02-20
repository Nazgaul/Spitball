<template>
    <v-row class="marketingActions pa-4 mb-2 mb-sm-4 text-center" dense>
        <v-col class="pa-0 mb-6 d-flex justify-space-between" cols="12">
            <div class="text text-left">{{$t($route.name === 'marketing' ? 'marketing_title' : 'promote_title')}}</div>
            <slot name="closeIcon"></slot>
        </v-col>
        
        <template v-for="(data, index) in resource">
            <actions :key="index" :data="data"></actions>
        </template>
    </v-row>
</template>

<script>
import actions from './action.vue';

export default {
    name: "marketingActions",
    components: {
        actions
    },
    props: {
        resource: {
            type: Object,
            required: true
        }
    },
}
</script>

<style lang="less">
    @import '../../../../styles/mixin.less';
    @import '../../../../styles/colors.less';
    .marketingActions {
        background: white;
        box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.15);
        border-radius: 8px;

        @media (max-width: @screen-xs) {
            box-shadow: none;
        }

        .text {
            color: @global-purple;
            font-weight: 600;
            font-size: 20px;
            @media (max-width: @screen-xs) {
                font-size: 16px;
            }
        }
    }
</style>