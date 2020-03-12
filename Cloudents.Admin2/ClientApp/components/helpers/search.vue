<template>
    <v-card color="red lighten-2"
            dark>
        <v-card-title class="headline red lighten-3">
            Search for {{this.context}}
        </v-card-title>
        
        <v-card-text>
            <v-combobox v-model="searchValue.name"
                            single-line
                            :loading="isLoading"
                            :search-input.sync="search"
                            color="white"
                            hide-no-data
                            hide-selected
                            item-text="Description"
                            item-value="API"
                            placeholder="Start typing to Search"
                            prepend-icon="mdi-database-search"
                            append-icon=" "
                            ></v-combobox>
        </v-card-text>
        <v-divider></v-divider>
        <v-expand-transition>
            <v-list v-if="items.length > 0" class="red lighten-3">
                <v-list-tile v-for="(field, i) in items"
                             :key="i"
                             @click="setCallback(field)">

                    <v-list-tile-content>
                        <v-list-tile-title> {{field.name}} | {{field.country}}</v-list-tile-title>
                        <!--<v-list-tile-sub-title v-text="field.key"></v-list-tile-sub-title>-->
                    </v-list-tile-content>
                </v-list-tile>
            </v-list>
        </v-expand-transition>
    </v-card>
</template>


<script>
    import { connectivityModule } from '../../services/connectivity.module';
    import debounce from 'lodash/debounce';

    export default {
        data: () => ({
        descriptionLimit: 60,
        entries: [],
        isLoading: false,
        model: '',
        search: null
  }),

    computed: {
        fields() {
            return this.model;
    },
        items: {
            get() {
                return this.entries;
            },
            set(val) {
                this.entries = val;
            }
          
    }
        },
    methods: {
        setCallback(item) {
            if (this.context === 'Course') {
                this.callback(item)
            }
            else {
                this.callback(item)
            }
        }
    },
        props: {
            callback: {
                type: Function,
                required: true
            },
            searchValue: {
                type: Object,
                //deafult: ""
            },
            contextCallback: {
                type: Function,
                required: true
            },
            context: {
                type: String,
                deafult: ""
            }
        },
    watch: {
        search: debounce(
            function (val) {
                
    // Items have already been loaded
    //if (this.items.length > 0) return
    
    // Items have already been requested
    if (this.isLoading) return

    this.isLoading = true

    // Lazily load input items
            
            return connectivityModule.http.get(`Admin${this.context}/search?${this.context.toLowerCase()}=${val}`)
                .then(res => {
                    if (this.context === 'Course') {
                        this.items = [].concat(res.courses);
                    }
                    else {
                        this.items = [].concat(res.universities);
                    }
  })
          .catch(err => {
        console.log(err)
    })
    .finally(() => (this.isLoading = false))
    }
    ,300)
}
}
</script>