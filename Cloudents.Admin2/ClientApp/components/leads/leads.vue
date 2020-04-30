<template>
    <div class="adminLeads">
        <v-container fluid grid-list-sm>
            <v-layout row wrap>
                <v-flex xs12 v-for="(document, index) in leadsItems" :key="index">
                    <v-card class="elevation-5">
                        <v-card-text>
                            <div><span class="font-weight-bold pr-2">Id:</span> {{document.id}}</div>
                            <div><span class="font-weight-bold pr-2">Name:</span> {{document.name}}</div>
                            <div><span class="font-weight-bold pr-2">Course Name:</span> {{document.course}}</div>
                            <div><span class="font-weight-bold pr-2">Email:</span> {{document.email}}</div>
                            <div><span class="font-weight-bold pr-2">Phone:</span> {{document.phone}}</div>
                            <div><span class="font-weight-bold pr-2">Text:</span> {{document.text}}</div>
                            <div><span class="font-weight-bold pr-2">Date:</span>{{document.dateTime.toLocaleString()}}</div>
                        </v-card-text>
                    </v-card>
                </v-flex>
            </v-layout>
            <div class="adminLeads-details" v-if="isItems">
                No Data
            </div>
        </v-container>
  </div>
</template>
<script>
import { getAdminLeads } from './leadsService'
import purchasedDocItem from "../userMainView/helpers/purchasedDocItem.vue";

export default {
    components: {
        purchasedDocItem
    },
    data() {
        return {
            leadsItems: [],
            isItems: false
        }
    },
    methods: {
        getLeads() {
            getAdminLeads().then(leads => {  
                if(leads.length > 0) {
                    this.leadsItems = leads  
                } else {
                    this.isItems = true
                }                
            }, err=>{
                this.isItems = false
                //TODO Error Toaster
            })
        }
    },
    created() {
        this.getLeads()
    }
}
</script>
