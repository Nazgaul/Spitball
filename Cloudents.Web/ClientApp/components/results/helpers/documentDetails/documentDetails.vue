<template>
    <v-flex grow class="details-row" v-show="item.university || item.course || item.professor">
        <div class="details-wrap">
                        <span class="aligned">{{item.university}}
                        <span :class="{'swap': isRtl}" class="sb-icon-arrow" v-show="item.university && item.course">&#10625;</span>
                        </span>
            <span class="aligned">{{item.course}}
                        <span :class="{'swap': isRtl}" class="sb-icon-arrow" v-show="item.professor">&#10625;</span>
                        </span>
            <span class="aligned">{{item.professor}}</span>
        </div>
    </v-flex>
</template>

<script>
    export default {
        name: "documentDetails",
        data() {
            return {
                isRtl: global.isRtl
            }    
        },
        props: {
            item: {
                type: Object,
                default: {}
            },
        },
        computed: {
            isVisible(){
                this.item.university || this.item.course || this.item.professor
            }

        },
    }
</script>
<style src="./documentDetails.less" lang="less"></style>

