<template>
    <v-data-table
      :headers="headers"
      :items="items"
      hide-default-header
      :selectable-key="'type'"
      single-select
      class="promoteTable"
      :footer-props="{
          showFirstLastPage: false,
          firstIcon: '',
          lastIcon: '',
          prevIcon: 'sbf-arrow-left-carousel',
          nextIcon: 'sbf-arrow-right-carousel',
          itemsPerPageOptions: [5]
      }">
        <template v-slot:header="{props}">
            <thead>
                <tr>
                    <th class="thHeaders" v-for="(header, index) in props.headers" :key="index">
                        <span>{{header.text}}</span>
                    </th>
                </tr>
            </thead>
        </template>

        <template v-slot:item="props">
            <tr @click="selectVideo(props)">
                <td class="product pa-0">
                    <div class="wrap d-flex pa-2">
                        <v-radio-group class="mr-n2 mt-5" :value="props.item.id === selectedId ? `radio-${props.index}` : ''" columns>
                            <v-radio :value="`radio-${props.index}`" on-icon="sbf-radioOn" off-icon="sbf-radioOff"></v-radio>
                        </v-radio-group>
                      <div class="d-flex align-center">
                        <img :src="$proccessImageUrl(props.item.preview, 120, 68)" alt="">
                      </div>
                      <div class="description ml-2">
                        <div class="intro text-truncate mb-1">
                          Sociology-A-Very-Short-Introduction 
                        </div>
                        <div class="course text-truncate">
                          Course: Economics
                        </div>
                      </div>
                  </div>
                </td>
                <td class="insideBox">
                  <div class="">{{props.item.type}}</div>
                </td>
                <td class="insideBox">
                  <div class="">{{props.item.likes}}</div>
                </td>
                <td class="insideBox">
                  <div class="">{{props.item.views}}</div>
                </td>
                <td class="insideBox">
                  <div class="">{{props.item.downloads}}</div>
                </td>
                <td class="insideBox">
                  <div class="">{{props.item.price}}</div>
                </td>
                <td class="insideBox">
                  <div class="">{{$d(new Date(props.item.date), 'tableDate')}}</div>
                </td>
            </tr>
        </template>

    </v-data-table>
</template>
<script>
import storeService from '../../../../services/store/storeService';
import marketingStore from '../../../../store/marketingStore'

export default {
  props: {
    dataType: {
      type: String,
      default: ''
    }
  },
  data() {
    return {
      selected: '',
      selectedId: -1,
      items: [],
      headers: [
        {
          text: 'Product',
          align: 'left',
          sortable: false,
          value: 'product',
        },
        { text: 'Type', value: 'type' },
        { text: 'Likes', value: 'likes' },
        { text: 'Views', value: 'views' },
        { text: 'Downloads', value: 'downloads' },
        { text: 'Price', value: 'price' },
        { text: 'Date', value: 'date' }
      ],
    }
  },
  methods: {
    selectVideo(props) {
      this.selected = `radio-${props.item.id}`;
      this.selectedId = props.item.id;
      this.$emit('selectedVideo', props.item);
    },
    getDataTable() {
      this.$store.dispatch('getPromoteData').then(items => {
        this.items = items.filter(item => item.type === this.dataType)
      }).catch(ex => {
        this.$appInsights.trackException({exception: new Error(ex)});
      })
    }
  },
  beforeDestroy() {
    storeService.unregisterModule(this.$store, 'marketingStore');
  },
  created() {
    storeService.registerModule(this.$store, 'marketingStore', marketingStore);
    this.getDataTable()
  }
}
</script>
<style lang="less">
@import '../../../../styles/colors';

.promoteTable {
  color: @global-purple !important;
  .thHeaders {
    color: @global-purple !important;
    font-weight: normal; // vuetify
    font-size: 14px;
    &:first-child {
      padding-left: 48px;
    }
  }
  .box {
    flex-shrink: 0;
  }
  .description {
    font-size: 12px;
    margin-top: 6px;
    width: 230px;
    .intro {
      font-weight: 600;
      font-size: 12px;

    }
    .course {
      font-size: 12px;
    }
  }
  .insideBox {
    padding: 14px;
    vertical-align: baseline;
  }
  .v-data-footer {
    padding: 0;
    font-size: 14px;
    .v-data-footer__select,
    .v-data-footer__pagination {
        color: @global-purple;
        font-size: 14px;
        opacity: .6;
    }
    .v-data-footer__icons-before,
    .v-data-footer__icons-after {
        .sbf-arrow-right-carousel, .sbf-arrow-left-carousel {
            transform: none /*rtl:rotate(180deg)*/;
            color: @global-purple !important; //vuetify
            font-size: 14px;
        }
    }
  }
}
</style>
