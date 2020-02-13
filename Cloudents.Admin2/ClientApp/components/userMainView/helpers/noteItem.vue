
<template>
    <div class="item-wrap" data-app>
        <v-card class="elevation-0 itemCard">
            <v-card-title class="itemCard_title grey lighten-2">
                <div><b>Email: </b>{{note.adminUser}}</div>
                <div><b>Created: </b>{{note.created}}</div>
            </v-card-title>
            <v-card-text>
                <b>Text: </b>{{note.text}}
            </v-card-text>
        </v-card>
    </div>
</template>
<script>
    export default {
        name: "noteItem",
        props: {
            note: {
                required: true
            },
        },
         methods: {
        }
    }
</script>
<style lang="less">
.item-wrap{
    .itemCard{
        border: 1px solid #ccc !important;
        .itemCard_title{
            padding: 10px 16px;
            justify-content: space-between;
        }
    }
}
</style>