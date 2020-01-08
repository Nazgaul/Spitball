<template>
    <div class="couponTable mt-5">
        
        <v-data-table
            :headers="headers"
            :items="coupons"
            class="couponTable_table"
            disable-initial-sort
            :loading="loading"
            expand
            :rows-per-page-items="[5, 10, 25,{text: 'All', value:-1}]">

            <template slot="items" slot-scope="props">
                <td>{{props.item.code}}</td>
                <td>{{props.item.couponType}}</td>
                <td>{{props.item.value}}</td>
                <td>{{props.item.owner}}</td>
                <td>{{props.item.tutorId}}</td>
                <td>{{props.item.description}}</td>
                <td>{{props.item.amountOfUsers}}</td>
            </template>
        </v-data-table>
        
    </div>
</template>

<script>
import couponService from "./couponService";

export default {
    name: 'couponTable',
    data() {
        return {
            headers: [
                {text: 'Code', value: 'code', sortable: true},
                {text: 'Coupont Type', value: 'couponType', sortable: false},
                {text: 'Value', value: 'value', sortable: false},
                {text: 'Owner', value: 'owner', sortable: false},
                {text: 'Tutor Id', value: 'tutorId', sortable: false},
                {text: 'Description', value: 'description', sortable: false},
                {text: 'Amount Users', value: 'amountOfUsers', sortable: false},
            ],
            coupons: [],
            loading: false,
        }
    },
    methods: {
        getCoupon() {
            this.loading = true;
            couponService.getCoupons().then(res => {
                console.log(res);
                this.coupons = res;
            }).catch(ex => {
                console.log(ex);
            }).finally(() => {
                this.loading = false;
            })
        }
    },
    created() {
        this.getCoupon()
    }
}
</script>