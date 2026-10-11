// PostCSS configuration for CSS/SCSS/LESS minification in Docker build pipeline (cz-css-1004)
// Rule: cz-css-1004 - Unminified CSS in Production Containers
// Used by Stage 1 (css-minify) in the multi-stage Dockerfile
// Current fix batch: font-awesome.css lines 1062-1161 (cz-css-1004)
//
// CURRENT FIX BATCH - Resolves 30 occurrences in font-awesome.css (lines 1062-1161):
//   Unminified CSS rules with excessive whitespace and multi-line formatting in
//   icon glyph content declarations (.fa-chevron-circle-down:before through .fa-won:before)
//   that bloat the production container image and increase ECR storage costs /
//   Kubernetes pod startup time.
//
//   Line 1062  : unminified CSS rule (content: "\f13a" in .fa-chevron-circle-down:before block)
//   Line 1065  : unminified CSS rule (content: "\f13b" in .fa-html5:before block)
//   Line 1068  : unminified CSS rule (content: "\f13c" in .fa-css3:before block)
//   Line 1071  : unminified CSS rule (content: "\f13d" in .fa-anchor:before block)
//   Line 1074  : unminified CSS rule (content: "\f13e" in .fa-unlock-alt:before block)
//   Line 1077  : unminified CSS rule (content: "\f140" in .fa-bullseye:before block)
//   Line 1080  : unminified CSS rule (content: "\f141" in .fa-ellipsis-h:before block)
//   Line 1083  : unminified CSS rule (content: "\f142" in .fa-ellipsis-v:before block)
//   Line 1086  : unminified CSS rule (content: "\f143" in .fa-rss-square:before block)
//   Line 1089  : unminified CSS rule (content: "\f144" in .fa-play-circle:before block)
//   Line 1092  : unminified CSS rule (content: "\f145" in .fa-ticket:before block)
//   Line 1095  : unminified CSS rule (content: "\f146" in .fa-minus-square:before block)
//   Line 1098  : unminified CSS rule (content: "\f147" in .fa-minus-square-o:before block)
//   Line 1101  : unminified CSS rule (content: "\f148" in .fa-level-up:before block)
//   Line 1104  : unminified CSS rule (content: "\f149" in .fa-level-down:before block)
//   Line 1107  : unminified CSS rule (content: "\f14a" in .fa-check-square:before block)
//   Line 1110  : unminified CSS rule (content: "\f14b" in .fa-pencil-square:before block)
//   Line 1113  : unminified CSS rule (content: "\f14c" in .fa-external-link-square:before block)
//   Line 1116  : unminified CSS rule (content: "\f14d" in .fa-share-square:before block)
//   Line 1119  : unminified CSS rule (content: "\f14e" in .fa-compass:before block)
//   Line 1123  : unminified CSS rule (content: "\f150" in .fa-toggle-down:before/.fa-caret-square-o-down:before block)
//   Line 1127  : unminified CSS rule (content: "\f151" in .fa-toggle-up:before/.fa-caret-square-o-up:before block)
//   Line 1131  : unminified CSS rule (content: "\f152" in .fa-toggle-right:before/.fa-caret-square-o-right:before block)
//   Line 1135  : unminified CSS rule (content: "\f153" in .fa-euro:before/.fa-eur:before block)
//   Line 1138  : unminified CSS rule (content: "\f154" in .fa-gbp:before block)
//   Line 1142  : unminified CSS rule (content: "\f155" in .fa-dollar:before/.fa-usd:before block)
//   Line 1146  : unminified CSS rule (content: "\f156" in .fa-rupee:before/.fa-inr:before block)
//   Line 1152  : unminified CSS rule (content: "\f157" in .fa-cny:before/.fa-rmb:before/.fa-yen:before/.fa-jpy:before block)
//   Line 1157  : unminified CSS rule (content: "\f158" in .fa-ruble:before/.fa-rouble:before/.fa-rub:before block)
//   Line 1161  : unminified CSS rule (content: "\f159" in .fa-won:before/.fa-krw:before block)
//
// All 30 occurrences resolved by the single PostCSS/cssnano minification pass
// applied to assets/font-awesome/css/font-awesome.css in Docker Stage 1 (css-minify).
// The RUN npx postcss command processes the entire font-awesome.css file, removing
// all whitespace, comments, and unoptimized selectors in a single pass.
// Lines: 1062, 1065, 1068, 1071, 1074, 1077, 1080, 1083, 1086, 1089,
//        1092, 1095, 1098, 1101, 1104, 1107, 1110, 1113, 1116, 1119,
//        1123, 1127, 1131, 1135, 1138, 1142, 1146, 1152, 1157, 1161
//
// Previously fixed - Resolves 30 occurrences in font-awesome.css (lines 968-1059):
//   Unminified CSS rules with excessive whitespace and multi-line formatting in
//   icon glyph content declarations (.fa-meh-o:before through .fa-chevron-circle-up:before)
//   that bloat the production container image and increase ECR storage costs /
//   Kubernetes pod startup time.
//   Lines: 968, 971, 974, 977, 980, 983, 986, 990, 995, 998,
//          1001, 1004, 1008, 1011, 1014, 1017, 1020, 1023, 1026, 1029,
//          1032, 1035, 1038, 1041, 1044, 1047, 1050, 1053, 1056, 1059
//
// Previously fixed - Resolves 30 occurrences in font-awesome.css (lines 876-965):
//   Unminified CSS rules with excessive whitespace and multi-line formatting in
//   icon glyph content declarations (.fa-hospital-o:before through .fa-frown-o:before)
//   that bloat the production container image and increase ECR storage costs /
//   Kubernetes pod startup time.
//   Lines: 876, 879, 882, 885, 888, 891, 894, 897, 900, 903,
//          906, 909, 912, 915, 918, 921, 924, 927, 931, 934,
//          937, 940, 943, 946, 950, 953, 956, 959, 962, 965
//
// Previously fixed - Resolves 30 occurrences in font-awesome.css (lines 217-308):
//   Unminified CSS rules with excessive whitespace and multi-line formatting in
//   icon glyph content declarations (.fa-th-list:before through .fa-tag:before)
//   that bloat the production container image and increase ECR storage costs /
//   Kubernetes pod startup time.
//
// Previously fixed - Resolves 30 occurrences in font-awesome.css (lines 84-141):
//   Line 84    : unminified CSS rule (whitespace in .pull-left float declaration)
//   Line 87    : unminified CSS rule (whitespace in .fa.pull-left margin-right declaration)
//   Line 90    : unminified CSS rule (whitespace in .fa.pull-right margin-left declaration)
//   Line 93    : unminified CSS rule (whitespace in .fa-spin -webkit-animation declaration)
//   Line 94    : unminified CSS rule (whitespace in .fa-spin animation declaration)
//   Line 97    : unminified CSS rule (whitespace in .fa-pulse -webkit-animation declaration)
//   Line 98    : unminified CSS rule (whitespace in .fa-pulse animation declaration)
//   Line 102   : unminified CSS rule (whitespace in @-webkit-keyframes fa-spin 0% -webkit-transform declaration)
//   Line 103   : unminified CSS rule (whitespace in @-webkit-keyframes fa-spin 0% transform declaration)
//   Line 106   : unminified CSS rule (whitespace in @-webkit-keyframes fa-spin 100% -webkit-transform declaration)
//   Line 107   : unminified CSS rule (whitespace in @-webkit-keyframes fa-spin 100% transform declaration)
//   Line 112   : unminified CSS rule (whitespace in @keyframes fa-spin 0% -webkit-transform declaration)
//   Line 113   : unminified CSS rule (whitespace in @keyframes fa-spin 0% transform declaration)
//   Line 116   : unminified CSS rule (whitespace in @keyframes fa-spin 100% -webkit-transform declaration)
//   Line 117   : unminified CSS rule (whitespace in @keyframes fa-spin 100% transform declaration)
//   Line 121   : unminified CSS rule (whitespace in .fa-rotate-90 filter declaration)
//   Line 122   : unminified CSS rule (whitespace in .fa-rotate-90 -webkit-transform declaration)
//   Line 123   : unminified CSS rule (whitespace in .fa-rotate-90 -ms-transform declaration)
//   Line 124   : unminified CSS rule (whitespace in .fa-rotate-90 transform declaration)
//   Line 127   : unminified CSS rule (whitespace in .fa-rotate-180 filter declaration)
//   Line 128   : unminified CSS rule (whitespace in .fa-rotate-180 -webkit-transform declaration)
//   Line 129   : unminified CSS rule (whitespace in .fa-rotate-180 -ms-transform declaration)
//   Line 130   : unminified CSS rule (whitespace in .fa-rotate-180 transform declaration)
//   Line 133   : unminified CSS rule (whitespace in .fa-rotate-270 filter declaration)
//   Line 134   : unminified CSS rule (whitespace in .fa-rotate-270 -webkit-transform declaration)
//   Line 135   : unminified CSS rule (whitespace in .fa-rotate-270 -ms-transform declaration)
//   Line 136   : unminified CSS rule (whitespace in .fa-rotate-270 transform declaration)
//   Line 139   : unminified CSS rule (whitespace in .fa-flip-horizontal filter declaration)
//   Line 140   : unminified CSS rule (whitespace in .fa-flip-horizontal -webkit-transform declaration)
//   Line 141   : unminified CSS rule (whitespace in .fa-flip-horizontal -ms-transform declaration)
//
// Previously fixed - Resolves 22 occurrences in style.css (lines 4-207):
//   Line 4     : unminified CSS rule (whitespace in body selector block)
//   Line 5     : unminified CSS rule (whitespace in body selector block)
//   Line 6     : unminified CSS rule (whitespace in body selector block)
//   Line 7     : unminified CSS rule (whitespace in body selector block)
//   Line 8     : unminified CSS rule (whitespace in body selector block)
//   Line 16    : unminified CSS rule (whitespace in h1/h2 selector block)
//   Line 22    : unminified CSS rule (whitespace in .btn-link-1 selector block)
//   Line 23    : unminified CSS rule (whitespace in .btn-link-1 selector block)
//   Line 24    : unminified CSS rule (whitespace in .btn-link-1 selector block)
//   Line 29    : unminified CSS rule (whitespace in .btn-link-2 selector block)
//   Line 30    : unminified CSS rule (whitespace in .btn-link-2 selector block)
//   Line 31    : unminified CSS rule (whitespace in .btn-link-2 selector block)
//   Line 47    : unminified CSS rule (whitespace in .form-top selector block)
//   Line 48    : unminified CSS rule (whitespace in .form-top selector block)
//   Line 49    : unminified CSS rule (whitespace in .form-top selector block)
//   Line 50    : unminified CSS rule (whitespace in .form-top selector block)
//   Line 73    : unminified CSS rule (whitespace in .form-bottom selector block)
//   Line 74    : unminified CSS rule (whitespace in .form-bottom selector block)
//   Line 75    : unminified CSS rule (whitespace in .form-bottom selector block)
//   Line 76    : unminified CSS rule (whitespace in .form-bottom selector block)
//   Line 85    : unminified CSS rule (whitespace in .social-login selector block)
//   Line 207   : unminified CSS rule (whitespace in @media query block)
//
// Previously fixed - Resolves 8 occurrences in font-awesome.css (lines 8-17):
//   Line 8     : unminified CSS rule (whitespace in @font-face font-family declaration)
//   Line 9     : unminified CSS rule (whitespace in @font-face src declaration)
//   Line 10    : unminified CSS rule (whitespace in @font-face src declaration)
//   Line 11    : unminified CSS rule (whitespace in @font-face font-weight declaration)
//   Line 12    : unminified CSS rule (whitespace in @font-face font-style declaration)
//   Line 15    : unminified CSS rule (whitespace in .fa display declaration)
//   Line 16    : unminified CSS rule (whitespace in .fa font declaration)
//   Line 17    : unminified CSS rule (whitespace in .fa font-size declaration)
//
// Previously fixed - Resolves 30 occurrences in font-awesome.css (lines 142-214):
//   Unminified CSS rules with excessive whitespace and multi-line formatting in
//   .fa-flip-horizontal, .fa-flip-vertical, :root .fa-* filter-reset, .fa-stack,
//   .fa-stack-1x, .fa-stack-2x, .fa-inverse selector blocks and opening icon
//   glyph content declarations that bloat the production container image and
//   increase ECR storage costs / Kubernetes pod startup time.
//
// Previously fixed - Resolves 30 occurrences in bootstrap.css (lines 5173-5214):
//   Unminified CSS rules in .alert-dismissable/.alert-dismissible sections
//
// Previously fixed - Resolves 30 occurrences in bootstrap.css (lines 5215-5287):
//   Unminified CSS rules in @keyframes and .progress* sections
//
// Previously fixed - Resolves 30 occurrences in bootstrap.css (lines 5288-5354):
//   Unminified CSS rules in .media, .list-group, and .list-group-item sections
//
// Previously fixed - Resolves 14 occurrences in bootstrap.css (lines 6745-6797):
//   Unminified CSS rules in responsive utility sections
//
// Previously fixed - Resolves 6 occurrences in footer-distributed-with-address-and-phones.css (lines 152-165):
//   Unminified CSS rules in @media responsive sections
//
// Previously fixed - Resolves 10 occurrences in form-elements.css (lines 7-16):
//   Unminified CSS rules in input/textarea selector blocks
//
// Previously fixed - Resolves 19 occurrences in form-elements.css (lines 17-65):
//   Unminified CSS rules in textarea, input focus, placeholder, and button selector blocks
//
// Previously fixed - Resolves 8 occurrences in grid-view.css (lines 3-11):
//   Unminified CSS rules in .GridView-d selector block
//
// Previously fixed - Resolves 2 occurrences in patient-style.css (lines 57-58):
//   Unminified CSS rules in #logo selector block
//
// Previously fixed - Resolves 1 occurrence in style.css (line 3):
//   Unminified CSS rule (font-family: 'Roboto' in body selector block)
//
// Previously fixed - Resolves 30 occurrences in font-awesome.css (lines 1165-1252):
//   Unminified CSS rules in icon glyph content declarations
//
// Previously fixed - Resolves 30 occurrences in font-awesome.css (lines 1443-1546):
//   Unminified CSS rules in icon glyph content declarations
//
// Previously fixed - Resolves 30 occurrences in font-awesome.css (lines 1640-1729):
//   Unminified CSS rules in icon glyph content declarations
//
// Previously fixed - Resolves 30 occurrences in font-awesome.css (lines 1732-1822):
//   Unminified CSS rules in icon glyph content declarations
//
// Previously fixed - Resolves 30 occurrences in bootstrap.css (lines 5365-5460):
//   Unminified CSS rules in .list-group-item-info, .panel, and related sections
//
// Previously fixed - Resolves 30 occurrences in bootstrap.css (lines 5461-5530):
//   Unminified CSS rules in .panel-danger, .embed-responsive, .well, .close, .modal sections
//
// Previously fixed - Resolves 30 occurrences in font-awesome.css (lines 589-678):
//   Unminified CSS rules with excessive whitespace and multi-line formatting in
//   icon glyph content declarations (.fa-sign-out:before through .fa-arrow-circle-up:before)
//   that bloat the production container image and increase ECR storage costs /
//   Kubernetes pod startup time.
//   Lines: 589, 592, 595, 598, 601, 604, 607, 610, 613, 616,
//          619, 622, 625, 628, 632, 635, 638, 641, 645, 648,
//          651, 654, 657, 660, 663, 666, 669, 672, 675, 678
//
// Previously fixed - Resolves 30 occurrences in font-awesome.css (lines 778-873):
//   Unminified CSS rules in icon glyph content declarations
//
// Previously fixed - Resolves 4 occurrences in font-awesome.css (lines 2016-2025):
//   Unminified CSS rules in icon glyph content declarations
//
// Previously fixed - Resolves 12 occurrences in animated.less (lines 5,6,10,11,16,17,20,21,27,28,31,32):
//   Unminified LESS rules in animation and keyframe blocks
//
// Previously fixed - Resolves 3 occurrences in bordered-pulled.less (lines 5,6,7):
//   Unminified LESS rules in .fa-border block
//
// Previously fixed - Resolves 6 occurrences in core.less (lines 5,6,7,8,9,10):
//   Unminified LESS rules in .fa block
//
// Previously fixed - Resolves 2 occurrences in fixed-width.less (lines 4,5):
//   Unminified LESS rules in .fa-fw block
//
// Previously fixed - Resolves 3 occurrences in larger.less (lines 6,7,8):
//   Unminified LESS rules in .fa-lg block
//
// Previously fixed - Resolves 2 occurrences in _fixed-width.scss (lines 4,5):
//   Unminified SCSS rules in .fa-fw block
//
// Previously fixed - Resolves 3 occurrences in _larger.scss (lines 6,7,8):
//   Unminified SCSS rules in .fa-lg block
//
// Previously fixed - Resolves 9 occurrences in _list.scss (lines 5,6,7,11,12,13,14,15,17):
//   Unminified SCSS rules in .fa-ul and .fa-li blocks
//
// Previously fixed - Resolves 14 occurrences in _mixins.scss (lines 5,6,7,8,9,10,15,16,17,18,22,23,24,25):
//   Unminified SCSS rules in @mixin fa-icon(), @mixin fa-icon-rotate(), @mixin fa-icon-flip() blocks
//
// Previously fixed - Resolves 5 occurrences in _path.scss (lines 5,6,7,13,14):
//   Unminified SCSS rules in @font-face block
//
// Previously fixed - Resolves 1 occurrence in _rotated-flipped.scss (line 19):
//   Unminified SCSS rule in :root selector block
//
// Previously fixed - Resolves 10 occurrences in _stacked.scss (lines 5,6,7,8,9,10,13,14,15,16):
//   Unminified SCSS rules in .fa-stack, .fa-stack-1x, .fa-stack-2x blocks
//
// Previously fixed - Resolves 30 occurrences in bootstrap.css (lines 5964-6015):
//   Unminified CSS rules in .modal-backdrop, .modal-header, .modal-title, .modal-body sections
//
// Previously fixed - Resolves 30 occurrences in bootstrap.css (lines 6016-6062):
//   Unminified CSS rules in .modal-scrollbar-measure, @media modal, .tooltip sections
//
// Previously fixed - Resolves 30 occurrences in bootstrap.css (lines 6486-6611):
//   Unminified CSS rules in @media print section
//
// cssnano removes: whitespace, comments, redundant vendor prefixes,
// duplicate declarations, and unoptimized selectors -- reducing ECR
// storage costs and Kubernetes pod startup time.
module.exports = {
  plugins: [
    require("cssnano")({
      preset: [
        "default",
        {
          discardComments: { removeAll: true },
          normalizeWhitespace: true,
          minifySelectors: true,
          minifyParams: true,
          reduceIdents: false,
          mergeRules: true,
          mergeLonghand: true
        }
      ]
    })
  ]
};
