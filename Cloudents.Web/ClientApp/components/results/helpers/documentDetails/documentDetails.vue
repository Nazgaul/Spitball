<template>
    <v-flex grow class="details-row" v-show="item.university || item.course || item.professor">
        <div class="details-wrap">
                        <span class="aligned">{{item.university}}
                        <v-icon class="sb-icon-arrow" v-show="item.university && item.course">sbf-nav-arrow-right</v-icon>
                        </span>
            <span class="aligned">{{item.course}}
                        <v-icon class="sb-icon-arrow" v-show="item.professor">sbf-nav-arrow-right</v-icon>
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

