<template>
    <v-data-table
      :headers="headers"
      :items="items"
      single-select
      class="promoteTable"
      :mobile-breakpoint="0"
      :footer-props="{
        showFirstLastPage: false,
        firstIcon: '',
        lastIcon: '',
        prevIcon: 'sbf-arrow-left-carousel',
        nextIcon: 'sbf-arrow-right-carousel',
        itemsPerPageOptions: [5]
      }">

        <template v-slot:top>
          <div class="tableTitle d-block d-sm-none">
            {{$t('promote_choose')}} {{$t('promote_your_content')}}
          </div>
        </template>

        <template v-slot:header.code="{header}">{{$t(header.text)}}</template>
        <template v-slot:header.couponType="{header}">{{$t(header.text)}}</template>
        <template v-slot:header.value="{header}">{{$t(header.text)}}</template>
        <template v-slot:header.amountOfUsers="{header}">{{$t(header.text)}}</template>
        <template v-slot:header.createTime="{header}">{{$t(header.text)}}</template>
        <template v-slot:header.expiration="{header}">{{$t(header.text)}}</template>

        <template v-slot:item="props">
            <tr @click="selectDocument(props)">
                <td class="product pa-0">
                    <div class="wrap d-flex pa-2">
                        <v-radio-group class="me-n2 mt-5" :value="props.item.id === selectedId ? `radio-${props.index}` : ''" columns>
                            <v-radio :value="`radio-${props.index}`" on-icon="sbf-radioOn" off-icon="sbf-radioOff"></v-radio>
                        </v-radio-group>
                      <div class="d-flex align-center">
                        <img :src="$proccessImageUrl(props.item.preview, {width:120, height:68})" alt="">
                      </div>
                      <div class="description ms-2">
                        <div class="intro text-truncate mb-1">
                          {{props.item.name}}
                        </div>
                        <div class="course text-truncate">
                          <span>{{$t('dashboardPage_course')}}</span>
                          <span>{{props.item.course}}</span>
                        </div>
                      </div>
                  </div>
                </td>
                <td class="insideBox"><div class="">{{dataType === 'Video' ? $t('promote_table_video') : $t('promote_table_document')}}</div></td>
                <td class="insideBox"><div class="">{{props.item.likes}}</div></td>
                <td class="insideBox"><div class="">{{props.item.views}}</div></td>
                <td class="insideBox"><div class="">{{props.item.downloads}}</div></td>
                <td class="insideBox"><div class="">{{props.item.price}}</div></td>
                <td class="insideBox"><div class="">{{$d(new Date(props.item.date), 'tableDate')}}</div></td>
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
    },
    resource: {
      required: false
    }
  },
  data() {
    return {
      selected: '',
      selectedId: -1,
      items: [],
      headers: [
        { text: this.$t('promote_table_product'), sortable: false, value: 'product' },
        { text: this.$t('promote_table_type'), value: 'type', sortable: false },
        { text: this.$t('promote_table_likes'), value: 'likes' },
        { text: this.$t('promote_table_views'), value: 'views' },
        { text: this.$t('promote_table_downloads'), value: 'downloads' },
        { text: this.$t('promote_table_price'), value: 'price' },
        { text: this.$t('promote_table_date'), value: 'date' }
      ],
    }
  },
  methods: {
    selectDocument(props) {
      this.selected = `radio-${props.item.id}`;
      this.selectedId = props.item.id;
      this.$emit('selectedDocument', props.item);
    },
    getDataTable() {
      this.$store.dispatch('getPromoteData').then(items => {
        this.items = items.filter(item => item.type === this.dataType);
      }).catch(ex => {
        this.$appInsights.trackException(ex);
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
@import '../../../../styles/colors.less';
@import '../../../../styles/mixin.less';

.promoteTable {
  color: @global-purple !important;
  margin-top: 25px;
  .tableTitle {
    color: @global-purple;
    font-weight: 600;
    font-size: 20px;
    @media (max-width: @screen-xs) {
        font-size: 18px;
    }
  }
  .v-data-table-header {
    span {
      color: @global-purple !important;
      font-weight: normal; // vuetify
      font-size: 14px;
    }
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
        // color: @global-purple;
        font-size: 14px;
        opacity: .6;
    }
    .v-data-footer__icons-before,
    .v-data-footer__icons-after {
        .sbf-arrow-right-carousel, .sbf-arrow-left-carousel {
            // color: @global-purple !important; //vuetify
            font-size: 14px;
        }
    }
  }
}
</style>
