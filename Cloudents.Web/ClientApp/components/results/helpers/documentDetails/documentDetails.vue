<template>
    <v-flex grow class="details-row">
        <div class="details-wrap">
                        <span class="aligned">{{item.university}}
                        <v-icon class="sb-icon-arrow">sbf-nav-arrow-right</v-icon>
                        </span>
            <span class="aligned">{{item.course}}
                        <v-icon class="sb-icon-arrow">sbf-nav-arrow-right</v-icon>
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
    }
</script>
<style src="./documentDetails.less" lang="less"></style>

