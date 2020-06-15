<template>
    <v-row class="marketingActions pa-0 text-center">
        <v-col class="pa-0 mb-6 d-block d-sm-none justify-space-between" cols="12">
            <div class="text text-left" v-t="'marketing_title'"></div>
        </v-col>
        
        <template v-for="(data, index) in resource">
            <actionBox :key="index" :data="data" :len="resource.length" :isDashboard="$route.name === routeNames.Dashboard"></actionBox>
        </template>
    </v-row>
</template>

<script>
import * as routeNames from '../../../routes/routeNames'
import actionBox from './actionBox.vue';

export default {
    name: "marketingActions",
    components: {
        actionBox
    },
    props: {
        resource: {
            type: Object,
            required: true
        }
    },
    data() {
        return {
            routeNames
        }
    }
}
</script>

<style lang="less">
    @import '../../../styles/mixin.less';
    @import '../../../styles/colors.less';
    .marketingActions {
        width: 100%;
        margin: 0 auto;

        .text {
            color: @global-purple;
            font-weight: 600;
            .responsive-property(font-size, 20px, null, 16px);
        }

        .box {
            display: flex;
            flex-direction: column;
            align-items: center;
            justify-content: space-between;
            color: @global-purple;
            .text1 {
                font-size: 16px;
                font-weight: 600;
            }
            .text2 {
                font-size: 12px;
            }
            .marketingbtn {
                text-transform: initial;
                font-weight: 600;
                min-width: 120px;
                letter-spacing: normal;
                span {
                    margin-bottom: 2px;
                }
            }
            &:not(:last-child) {
                border-right: 1px solid #dddddd;

                @media (max-width: @screen-xs) {
                    border-bottom: 1px solid #dddddd;
                    border-right: none;
                }
            }
        }
    }
</style>