<template>
  <v-container grid-list-xl class="dashboardFilters pa-0 mb-4">
        <v-layout class="">

            <v-flex xs2>
                <v-combobox
                    v-model="course"
                    :items="courseItems"
                    label="All Courses"
                    :append-icon="'sbf-arrow-down'"
                ></v-combobox>
            </v-flex>
            
            <v-flex xs2>
                <v-combobox
                    v-model="type"
                    :items="typeItems"
                    label="Types"
                    :append-icon="'sbf-arrow-down'"
                ></v-combobox>
            </v-flex>

            <v-flex xs2>
                <v-combobox
                    v-model="status"
                    :items="statusItems"
                    label="Status"
                    :append-icon="'sbf-arrow-down'"
                ></v-combobox>
            </v-flex>
            
            <v-flex xs2>
                <v-combobox
                    v-model="date"
                    :items="dateItems"
                    label="Date"
                    :append-icon="'sbf-arrow-down'"
                ></v-combobox>
            </v-flex>

            <v-flex xs2>
                <v-text-field
                    v-model="searchText"
                    label="Search"
                >

                </v-text-field>
            </v-flex>
            
            <v-flex xs2 >
                <v-btn
                    color="primary"
                >
                    Make a change
                </v-btn>
            </v-flex>

        </v-layout>
    </v-container>
</template>

<script>
export default {
    name: 'dashboardFilters',
    components: {},
    data() {
        return {
            searchText: '',
            course: '',
            type: '',
            status: '',
            date: '',
            courseItems: [1,2,3,4],
            typeItems: [1,2,3,4],
            statusItems: [1,2,3,4],
            dateItems: [1,2,3,4],
        }
    }
}
</script>

<style lang="less">
    
</style>