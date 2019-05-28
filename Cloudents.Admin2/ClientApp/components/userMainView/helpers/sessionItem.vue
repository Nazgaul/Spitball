<template>
    <div class="item-wrap">
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
    </div>
</template>

<script>
    
   
    export default {
        name: "sessionItem",
        data () {
      return {
            headers: [
                    { text: 'Tutor' },
                    { text: 'Student' },
                    { text: 'Created' },
                    { text: 'Duration (min)' }
                ],
        }
    },
        props: {
            sessions: {
                type: Array,
                required: false
            },
            
        },

         methods: {
           
        }
    }


</script>

<style  lang="scss">
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