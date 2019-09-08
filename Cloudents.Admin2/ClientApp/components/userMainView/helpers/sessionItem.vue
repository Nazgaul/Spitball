<template>
    <!-- <div class="item-wrap">
            <v-data-table
                :items="sessions"
                class="elevation-1"
                hide-actions
                :headers="headers"
            >
                <template slot="items" slot-scope="props">
                <td class="text-xs-left">{{ props.item.tutor }}</td>
                <td class="text-xs-left">{{ props.item.student }}</td>
                <td class="text-xs-left">{{ props.item.created.toLocaleString('he-IL', {  
                    day : 'numeric',
                    month : 'short',
                    year : 'numeric'
                }) }}
                </td>
                <td class="text-xs-left">{{ props.item.duration }}</td>
               
                </template>
            </v-data-table>
    </div> -->

    <div class="item-wrap" data-app>
        <v-card class="elevation-5">
                <v-card-text>
                    <v-flex xs3><b>Tutor: </b>{{session.tutor}}</v-flex>
                    <v-flex xs3><b>Student: </b>{{session.student}}</v-flex>
                    <v-flex xs3><b>Created: </b>{{session.created.toLocaleString('en-US', {  
                    day : 'numeric',
                    month : 'short',
                    year : 'numeric'
                }) }}</v-flex>
                <v-flex xs3><b>Duration (min): </b>{{session.duration}}</v-flex>
                </v-card-text>
        </v-card>
    </div>
</template>

<script>
    
   
    export default {
        name: "sessionItem",
        data () {
      return {
        }
    },
        props: {
            session: {
                // type: Array,
                required: false
            },
            
        },

         methods: {
        }
    }


</script>

<style  lang="less">
    .item-wrap{
        .session-toolbar{
            max-width: 100%;
            background-color: transparent!important;
            box-shadow: none;
            border-bottom: 1px solid grey;
        }
        .v-card{
            max-width: 100%;
        }
    }
    .session-text-title{
        font-size: 15px;
        text-align: left;
    }
   

</style>