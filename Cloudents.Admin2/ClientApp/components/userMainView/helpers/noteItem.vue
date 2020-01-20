
<template>
    <div class="item-wrap" data-app>
        <v-card class="elevation-5">
                <v-card-text>
                    <v-flex xs3><b>Text: </b>{{note.text}}</v-flex>
                    <v-flex xs3><b>Admin Email: </b>{{note.adminUser}}</v-flex>
                    <v-flex xs3><b>Created: </b>{{note.created}}</v-flex>
                </v-card-text>
        </v-card>
    </div>
</template>
<script>
    export default {
        name: "noteItem",
        data () {
      return {
        }
    },
        props: {
            note: {
                // type: Array,
                required: true
            },
            
        },

         methods: {
        }
    }
</script>