<template>
    <v-flex grow class="details-row" v-show="isVisible">
        <div class="details-wrap">
                        <span class="aligned">{{item && item.university}}
                        <span :class="{'swap': isRtl}" class="sb-icon-arrow" v-show=" item && item.university && item.course">&#10625;</span>
                        </span>
            <span class="aligned">{{item && item.course}}
                        <span :class="{'swap': isRtl}" class="sb-icon-arrow" v-show="item && item.professor">&#10625;</span>
                        </span>
            <span class="aligned cut-long" v-line-clamp="'1'">{{item && item.professor}}</span>
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
            },
        },
        computed: {
            isVisible(){
                if(this.item){
                   return this.item.university || this.item.course || this.item.professor
                }
            }
        },
    }
</script>
<style src="./documentDetails.less" lang="less"></style>

